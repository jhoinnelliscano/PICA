
using eCommerce.Commons.Objects.Messaging;
using eCommerce.PublisherSubscriber.Contracts;
using eCommerce.PublisherSubscriber.Messaging;
using eCommerce.PublisherSubscriber.Object;
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
    public class ConsumerOrderMsg : ConsumerMessage
    {
        private IShoppingCartService _shoppingCartService;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ConsumerOrderMsg> _logger;

        public ConsumerOrderMsg(IServiceProvider serviceProvider, IMessagingFactory messagingFactory,
            ILogger<ConsumerOrderMsg> logger, IConfiguration configuration)
           : base(messagingFactory, configuration, logger)
        {
            _serviceProvider = serviceProvider;
            InitAsDistributedMessage(configuration.GetValue<string>("AppConfiguration:MQ:PurchaseOrderQueue"));
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
                    var message = JsonConvert.DeserializeObject<Message<OrderMsg>>(_message);

                    if (message != null)
                    {
                        foreach (var item in message.BusinessObject.Products)
                        {
                            var result = await _shoppingCartService.DeleteShoppingCartByUser(message.BusinessObject.CustomerId, item.ProductId);
                        }
                        var msgBody = new { message = _message };
                        _logger.LogInformation("ConsumerOrderMsg.Ok consumo de cola de ordenes {msgBody}", msgBody);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ConsumerOrderMsg.Error", new { message = _message });
            }
        }
    }
}
