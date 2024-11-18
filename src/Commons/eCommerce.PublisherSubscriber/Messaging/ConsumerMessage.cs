using eCommerce.PublisherSubscriber.Contracts;
using eCommerce.PublisherSubscriber.Object;
using eCommerce.PublisherSubscriber.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Text;

namespace eCommerce.PublisherSubscriber.Messaging
{
    public abstract class ConsumerMessage : BackgroundService, IConsumer
    {
        private readonly ILogger<ConsumerMessage> _logger;
        protected readonly IConnectionFactory _connectionFactory;
        protected string _queueName;
        protected readonly string _exchangeName;
        protected IModel _channel;
        protected IConnection _connection;
        protected string _message;

        public ConsumerMessage(IMessagingFactory messagingFactory, IConfiguration configuration, ILogger<ConsumerMessage> logger)
        {
            //_connectionFactory = new ConnectionFactory()
            //{
            //    HostName = configuration.GetValue<string>("AppConfiguration:MQ:Server"),
            //    VirtualHost = "/",
            //    Port = AmqpTcpEndpoint.UseDefaultPort,
            //    UserName = configuration.GetValue<string>("AppConfiguration:MQ:UserName"),
            //    Password = configuration.GetValue<string>("AppConfiguration:MQ:Password")
            //};
            _connectionFactory = messagingFactory.GetRabitMQFactory(configuration);
            _logger = logger;
        }

        public virtual void InitAsDistributedMessage(string exchangeName) 
        {
            try
            {
                _connection = _connectionFactory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.ExchangeDeclare(
                          exchange: exchangeName,
                          type: ExchangeType.Fanout);

                _queueName = _channel.QueueDeclare().QueueName;

                _channel.QueueBind(
                    queue: _queueName,
                    exchange: exchangeName,
                    routingKey: ""); // llenar en caso de usar un ExchangeType diferente a Fanout (debe ser igual al publisher)
            }
            catch (BrokerUnreachableException ex)
            {
                _logger.LogError(ex, $"No se puede conectar a RabbitMQ : {ex.Message}");
            }
        }

        public virtual void InitAsPublishedMessage(string queueName)
        {
            try
            {
                _queueName = queueName;
                _connection = _connectionFactory.CreateConnection();
                _channel = _connection.CreateModel();
                _channel.QueueDeclare(
                            queue: queueName,
                            durable: true,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null);
                _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
            }
            catch (BrokerUnreachableException ex)
            {
                _logger.LogError(ex, $"No se puede conectar a RabbitMQ : {ex.Message}");
            }
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }

        public string GetMessage() 
        {
            return _message;
        }

        public abstract Task InitServices(CancellationToken stoppingToken);
        public abstract Task ProcessMessage();

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            await base.StopAsync(stoppingToken);
        }
    }
}
