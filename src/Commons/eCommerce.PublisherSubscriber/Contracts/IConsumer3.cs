
using eCommerce.PublisherSubscriber.Object;
using eCommerce.PublisherSubscriber.Objects;

namespace eCommerce.PublisherSubscriber.Contracts
{
    public interface IConsumer3//<T>
    {
        Task ProcesarMensaje(MqMessage mensaje);
        //void InitAsPublishedMessage(string queueName);
        //void InitAsDistributedMessage(string exchangeName);
        //Task ProcessMessage(Message<T> messsage);
    }
}
