
namespace eCommerce.PublisherSubscriber.Objects
{
    public class MqMessage
    {
        public string Date { get; set; }
        public string Description { get; set; }
        public string Message { get; set; }

        public MqMessage() { }

        public MqMessage(string description, string message) 
        {
            Date = DateTime.Now.ToString();
            Description = description;
            Message = message;
        }
    }
}
