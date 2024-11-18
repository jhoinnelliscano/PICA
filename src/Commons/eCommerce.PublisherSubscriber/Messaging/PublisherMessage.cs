using eCommerce.PublisherSubscriber.Contracts;
using eCommerce.PublisherSubscriber.Object;
using eCommerce.PublisherSubscriber.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Text;

namespace eCommerce.PublisherSubscriber.Messaging
{
    public class PublisherMessage : IPublisher
    {
        private readonly IConnectionFactory _connectionFactory;
        private IModel _channel;
        private readonly ILogger<PublisherMessage> _logger;
        private readonly IConnection _connection;

        public PublisherMessage(IMessagingFactory messagingFactory, IConfiguration configuration, ILogger<PublisherMessage> logger) 
        {
            _logger = logger;
            try
            {
                _connectionFactory = messagingFactory.GetRabitMQFactory(configuration);
                _connection = _connectionFactory.CreateConnection();
                _channel = _connection.CreateModel();
            }
            catch (BrokerUnreachableException ex)
            {
                _logger.LogError(ex, $"No se puede conectar a RabbitMQ : {ex.Message}");
            }
        }

        public virtual void DistributeMessage(string message, string exchangeName)
        {
            try 
            {
                if (_channel.IsClosed)
                {
                    _channel = _connection.CreateModel();
                    _channel.ExchangeDeclare(
                        exchange: exchangeName,
                        type: ExchangeType.Fanout); //topic : guardar mensajes para consumidores fuera de linea
                }

            //    string content = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(message);

                _channel.BasicPublish(
                    exchange: exchangeName,
                    routingKey: "", // llenar en caso de usar otro ExchangeType diferente a Fanout
                    basicProperties: null,
                    body: body);
            }
            catch (BrokerUnreachableException ex)
            {
                _logger.LogError(ex, $"No se puede conectar a RabbitMQ : {ex.Message}");
            }           
        }

        public virtual void PublishMessage(string message, string queueName)
        {
            try
            {
                if (_channel.IsClosed)
                {
                    _channel = _connection.CreateModel();

                    _channel.QueueDeclare(
                        queue: queueName,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);
                }
                // string content = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(message);

                    var properties = _channel.CreateBasicProperties();
                    properties.Persistent = true;

                    _channel.BasicPublish(
                        exchange: "",
                        routingKey: queueName,
                        basicProperties: properties,
                        body: body);
              
            }
            catch (BrokerUnreachableException ex)
            {
                _logger.LogError(ex, $"No se puede conectar a RabbitMQ : {ex.Message}");
            }
        }
    }
}
