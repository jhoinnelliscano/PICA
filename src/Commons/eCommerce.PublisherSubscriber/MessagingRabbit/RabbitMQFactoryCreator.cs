using eCommerce.PublisherSubscriber.Contracts;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace eCommerce.PublisherSubscriber.MessagingRabbit
{
    public class RabbitMQFactoryCreator : IMessagingFactory
    {
        public IConnectionFactory GetRabitMQFactory(IConfiguration configuration)
        {
            return new ConnectionFactory()
            {
                HostName = configuration.GetValue<string>("AppConfiguration:MQ:Server"),
                VirtualHost = "/",
                Port = AmqpTcpEndpoint.UseDefaultPort,
                UserName = configuration.GetValue<string>("AppConfiguration:MQ:UserName"),
                Password = configuration.GetValue<string>("AppConfiguration:MQ:Password")
            };
        }
    }
}