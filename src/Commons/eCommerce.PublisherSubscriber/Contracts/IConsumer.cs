
using eCommerce.PublisherSubscriber.Object;


namespace eCommerce.PublisherSubscriber.Contracts
{
    public interface IConsumer
    {    
        void InitAsPublishedMessage(string queueName);
        void InitAsDistributedMessage(string exchangeName);
        string GetMessage();
        Task InitServices(CancellationToken stoppingToken);
        Task ProcessMessage();
    }
}
