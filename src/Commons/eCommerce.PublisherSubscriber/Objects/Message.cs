
namespace eCommerce.PublisherSubscriber.Object
{
    public class Message<T>
    {
        public string Description { get; set; }
        public T BusinessObject { get; set; }

        public Message(T businessObject) 
        {
            BusinessObject = businessObject;
        }

        public Message(string description, T businessObject)
        {
            Description = description;
            BusinessObject = businessObject;
        }

        public Message() { }
    }
}
