
namespace eCommerce.PublisherSubscriber.Objects
{
    public class MessageBody : IEquatable<MessageBody>
    {
        public string Body { get; set; }
        public bool Equals(MessageBody? other)
        {
            throw new NotImplementedException();
        }
    }
}
