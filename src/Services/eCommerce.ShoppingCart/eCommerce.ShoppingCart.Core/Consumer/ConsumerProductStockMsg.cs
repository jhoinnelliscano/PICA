using eCommerce.Commons.Objects.Messaging;
using eCommerce.PublisherSubscriber.Contracts;
using eCommerce.PublisherSubscriber.Messaging;
using eCommerce.PublisherSubscriber.Object;
using eCommerce.ShoppingCart.Core.Config;
using eCommerce.ShoppingCart.Core.Contracts.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace eCommerce.ShoppingCart.Core.Consumer
{
    public class ConsumerProductStockMsg : ConsumerMessage
    {
        private IShoppingCartService _shoppingCartService;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ConsumerProductStockMsg> _logger;

        public ConsumerProductStockMsg(IServiceProvider serviceProvider, IMessagingFactory messagingFactory,
            ILogger<ConsumerProductStockMsg> logger, IConfiguration configuration)
           : base(messagingFactory, configuration, logger)
        {
            _serviceProvider = serviceProvider;
            InitAsPublishedMessage(configuration.GetValue<string>("AppConfiguration:MQ:ProductStockUpdateQueue"));
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            EventingBasicConsumer consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (sender, eventArgs) =>
            {
                var content = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                _message = content;
                await InitServices(stoppingToken);
                await ProcessMessage();
                _channel.BasicAck(eventArgs.DeliveryTag, false);
            };
            _channel.BasicConsume(_queueName, false, consumer);

        }

        public override async Task InitServices(CancellationToken stoppingToken)
        {
            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                _shoppingCartService = scope.ServiceProvider.GetRequiredService<IShoppingCartService>();
            }
        }

        public override async Task ProcessMessage()
        {
            try
            {
                if (!string.IsNullOrEmpty(_message))
                {
                    var message = JsonConvert.DeserializeObject<Message<ProductUpdateStockMsg>>(_message);

                    if (message != null)
                    {
                        var stockByProduct = message.BusinessObject;
                        var result = await _shoppingCartService.UpdateShoppingCartStatus(stockByProduct.Id, stockByProduct.Available);
                        var msgBody = new { message = _message };
                        _logger.LogInformation("ConsumerProductStockMsg.Ok consumo de cola de cambio de stock en producto {msgBody}", msgBody);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ConsumerProductStockMsg.Error", new { message = _message });
            }
        }
    }
}
