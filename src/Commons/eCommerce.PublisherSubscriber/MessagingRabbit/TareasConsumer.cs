﻿
using eCommerce.PublisherSubscriber.Contracts;
using eCommerce.PublisherSubscriber.Objects;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace eCommerce.PublisherSubscriber.MessagingRabbit
{
    public class TareasConsumer : BackgroundService, IConsumer3
    {
        private readonly ILogger<TareasConsumer> _logger;
        private readonly IConnectionFactory _connectionFactory;
        private readonly string _queueName;
        private IModel _channel;
        private IConnection _connection;

        public TareasConsumer(IMessagingFactory messagingFactory, IConfiguration configuration, ILogger<TareasConsumer> logger)
        {
            _logger = logger;
         //   _connectionFactory = messagingFactory.GetRabitMQFactory("amqp://localhost");//(configuration.GetValue<string>("MQServer"));
            _connectionFactory = new ConnectionFactory()
            {
                HostName = "localhost", //"http://10.43.102.50",
                VirtualHost = "/",
               
                Port = AmqpTcpEndpoint.UseDefaultPort,
                UserName = "guest",
                Password = "guest"

            };
            _queueName = "NotificationQueue"; //configuration.GetValue<string>("queueName");
            Init();
        }

        private void Init()
        {
            try 
            {
                _connection = _connectionFactory.CreateConnection();
                _channel = _connection.CreateModel();
                _channel.QueueDeclare(
                        queue: _queueName,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null
                        );
                _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
            }
            catch (BrokerUnreachableException ex)
            {
                _logger.LogError(ex, $"No se puede conectar a RabbitMQ : {ex.Message}");
            }
            
        }


        public async Task ProcesarMensaje(MqMessage mensaje)
        {
//            _logger.LogInformation($"[Mensaje recibido] - [Fecha:{mensaje.FechaEnvio}] - [Tarea: {mensaje.Tarea.Id} {mensaje.Tarea.Name} {mensaje.Tarea.IsComplete}]");

            string fileName = "tareas.csv";
            string registro = $"{mensaje.Message.Body}";//$"{mensaje.FechaEnvio},{mensaje.Tarea.Id},{mensaje.Tarea.Name},{mensaje.Tarea.IsComplete}";

            using (StreamWriter writer = new StreamWriter(fileName, true))
            {
                await writer.WriteLineAsync(registro);
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            EventingBasicConsumer consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (sender, enventArgs) =>
            {
                string content = Encoding.UTF8.GetString(enventArgs.Body.ToArray());
                MqMessage mensaje = JsonConvert.DeserializeObject<MqMessage>(content);

                await ProcesarMensaje(mensaje);

                _channel.BasicAck(enventArgs.DeliveryTag, false);
            };

            _channel?.BasicConsume(_queueName, false, consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }
}
