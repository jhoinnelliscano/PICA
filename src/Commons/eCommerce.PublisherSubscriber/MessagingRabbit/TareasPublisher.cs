﻿using System;
using System.Text;
using RabbitMQ.Client;

using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;
using System.Threading.Tasks;
using RabbitMQ.Client.Exceptions;
using Microsoft.Extensions.Logging;
using eCommerce.PublisherSubscriber.Contracts;
using eCommerce.PublisherSubscriber.Objects;

namespace eCommerce.PublisherSubscriber.MessagingRabbit
{
    public class TareasPublisher : IPublisher3, IDisposable
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly IConnection _connection;
        private readonly ILogger<TareasPublisher> _logger;
        private IModel _channel;
        private readonly string _queueName;
        private readonly string _exchangeName;

        public TareasPublisher(IMessagingFactory messagingFactory, IConfiguration configuration, ILogger<TareasPublisher> logger) 
        {
            _logger = logger;
            _connectionFactory = messagingFactory.GetRabitMQFactory(configuration);
            try
            {
                _connection = _connectionFactory.CreateConnection();
                _channel = _connection.CreateModel();
            }
            catch (BrokerUnreachableException ex) 
            {
                _logger.LogError(ex, $"No se puede conectar a RabbitMQ : {ex.Message}");
            }
            
            _queueName = configuration.GetValue<string>("queueName");
            _exchangeName = configuration.GetValue<string>("exchangeName");
        }

        public void DistribuirMensaje(MqMessage mensaje)
        {
            if (_channel.IsClosed)
            {
                _channel = _connection.CreateModel();
                _channel.ExchangeDeclare(
                    exchange: _exchangeName,
                    type: ExchangeType.Fanout
                    );
            }

            string content = JsonConvert.SerializeObject(mensaje);
            var body = Encoding.UTF8.GetBytes(content);

            _channel.BasicPublish(
                exchange: _exchangeName,
                routingKey: "",
                basicProperties: null,
                body: body);
        }

        public void PublicarMensaje(MqMessage mensaje)
        {
            if (_channel.IsClosed)
            {
                _channel = _connection.CreateModel();
                _channel.QueueDeclare(
                    queue: _queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                    );
            }
            string content = JsonConvert.SerializeObject(mensaje);
            var body = Encoding.UTF8.GetBytes(content);
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;

            _channel.BasicPublish(
                exchange: "",
                routingKey: _queueName,
                basicProperties: properties,
                body: body);

        }

        public Task PublicarMensajeAsync(MqMessage mensaje)
        {
            return Task.Run(() => { 
                PublicarMensaje(mensaje);  
            });
        }

        public Task DistribuirMensajeAsync(MqMessage mensaje)
        {
            return Task.Run(() =>
            {
                DistribuirMensaje(mensaje);
            });
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}
