using eCommerce.Commons.Objects.Messaging;
using eCommerce.Notifications.Core.Config;
using eCommerce.Notifications.Core.Contracts.Services;
using eCommerce.Notifications.Core.Objects.Dtos;
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

namespace eCommerce.Notifications.Core.Consumers
{
    public class ConsumerNotificationMsg: ConsumerMessage
    {
        private  INotificationService _notificationService;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ConsumerNotificationMsg> _logger;

        public ConsumerNotificationMsg(IServiceProvider serviceProvider, IMessagingFactory messagingFactory,
            ILogger<ConsumerNotificationMsg> logger, IConfiguration configuration)
           : base(messagingFactory, configuration, logger)
        {
            _serviceProvider = serviceProvider;
            InitAsPublishedMessage(configuration.GetValue<string>("AppConfiguration:MQ:NotificationQueue"));
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
                var msgBody = new { message = _message };
                _logger.LogInformation("ConsumerNotificationMsg.Ok consumo de cola de notificación {msgBody}", msgBody);
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
                _notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
            }
        }

        public override async Task ProcessMessage()
        {
            try
            {
                if (!string.IsNullOrEmpty(_message))
                {
                    var message = JsonConvert.DeserializeObject<Message<NotificationMsg>>(_message);

                    if (message != null)
                        await _notificationService.SentNotification(new NotificationDto(message.BusinessObject.Subject,
                            message.BusinessObject.CustomerEmail, message.BusinessObject.Body, message.BusinessObject.CustomerName));
                    var msgBody = new { message = _message };
                    _logger.LogInformation("ConsumerNotificationMsg.Ok consumo de cola de notificación {msgBody}", msgBody);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ConsumerNotificationMsg.Error", new { message = _message });
            }
        }
    }
}
