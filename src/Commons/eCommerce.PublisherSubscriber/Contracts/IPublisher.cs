using eCommerce.PublisherSubscriber.Object;

namespace eCommerce.PublisherSubscriber.Contracts
{
    public interface IPublisher
    {
        void PublishMessage(string message, string queueName);
        void DistributeMessage(string message, string exchangeName);
    }
}
