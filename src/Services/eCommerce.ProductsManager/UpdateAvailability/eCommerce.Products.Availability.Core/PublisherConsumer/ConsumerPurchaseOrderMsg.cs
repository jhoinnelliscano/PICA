using eCommerce.Commons.Objects.Messaging;
using eCommerce.Products.Availability.Core.Config;
using eCommerce.Products.Availability.Core.Contracts.Services;
using eCommerce.Products.Availability.Core.Objects.Dtos;
using eCommerce.PublisherSubscriber.Contracts;
using eCommerce.PublisherSubscriber.Messaging;
using eCommerce.PublisherSubscriber.Object;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace eCommerce.Products.Availability.Core.PublisherConsumer
{
    public class ConsumerPurchaseOrderMsg : ConsumerMessage
    {
        private  IProductService _productService;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ConsumerPurchaseOrderMsg> _logger;

        public ConsumerPurchaseOrderMsg(IServiceProvider serviceProvider, IMessagingFactory messagingFactory,
            ILogger<ConsumerPurchaseOrderMsg> logger, IConfiguration configuration)
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
                _productService = scope.ServiceProvider.GetRequiredService<IProductService>();
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
                        var order = message.BusinessObject;

                        if (order != null && order.Products.Any())
                        {
                            foreach (var product in order.Products)
                            {
                                var result = await _productService.UdateProductStock(product.ProductId, product.Units);
                            }
                        }
                        var msgBody = new { message = _message };
                        _logger.LogInformation("ConsumerPurchaseOrderMsg.Ok consumo de cola de ordenes {msgBody}", msgBody);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ConsumerPurchaseOrderMsg.Error", new { message = _message });
            }
        }
    }
}
