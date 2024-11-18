
namespace eCommerce.Commons.Objects.Requests.Order
{
    public class CreateOrderRequest
    {
        public string CustomerId { get; set; } = null!;
        public string CustomerEmail { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public string? Comment { get; set; }
        public OrderShippingAddressRequest ShippingAddress { get; set; }
        public IEnumerable<OrderDetailRequest> OrderDetail { get; set; }

        public CreateOrderRequest(string orderId, string customerId, string customerEmail, string customerName, int statusId, string comment,
            string paymentRef, OrderShippingAddressRequest address, List<OrderDetailRequest> orderDetail)
        {
            CustomerId = customerId;
            CustomerEmail = customerEmail;
            CustomerName = customerName;
            Comment = comment;
            ShippingAddress = address;
            OrderDetail = orderDetail;
        }

        public CreateOrderRequest() { }
    }
}
