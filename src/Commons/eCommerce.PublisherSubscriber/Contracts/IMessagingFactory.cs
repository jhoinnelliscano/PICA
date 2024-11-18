
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace eCommerce.PublisherSubscriber.Contracts
{
    public interface IMessagingFactory
    {
        IConnectionFactory GetRabitMQFactory(IConfiguration configuration);
    }
}
