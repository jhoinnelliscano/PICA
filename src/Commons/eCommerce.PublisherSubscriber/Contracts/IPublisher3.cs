
using eCommerce.PublisherSubscriber.Object;
using eCommerce.PublisherSubscriber.Objects;

namespace eCommerce.PublisherSubscriber.Contracts
{
    public interface IPublisher3//<T>
    {
        //void PublishMessage(Message<T> message, string queueName);
        //void DistributeMessage(Message<T> message, string exchangeName);
        void PublicarMensaje(MqMessage mensaje);
        void DistribuirMensaje(MqMessage mensaje);
        Task PublicarMensajeAsync(MqMessage mensaje);
        Task DistribuirMensajeAsync(MqMessage mensaje);
    }
}
