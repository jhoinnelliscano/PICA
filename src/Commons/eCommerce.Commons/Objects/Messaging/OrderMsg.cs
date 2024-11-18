
namespace eCommerce.Commons.Objects.Messaging
{
    public class OrderMsg
    {
        public string CustomerId { get; set; }
        public IEnumerable<OrderProductMsg> Products { get; set; }


        public OrderMsg(string customerId, IEnumerable<OrderProductMsg> products)
        {
            CustomerId = customerId;
            Products = products;
        }

        public OrderMsg() { }
    }
}
