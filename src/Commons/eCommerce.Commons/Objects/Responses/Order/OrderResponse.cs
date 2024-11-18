
namespace eCommerce.Commons.Objects.Responses.Order
{
    public class OrderResponse
    {
        public string Id { get; set; } = null!;
        public string CustomerId { get; set; } = null!;
        public string CustomerEmail { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public int OrderStatusId { get; set; }
        public string OrderStatusDesc { get; set; }
        public string? Comment { get; set; }
        public string? PaymentRef { get; set; }
        public decimal Total { get; set; }
        public DateTime Date { get; set; }
        public OrderAddressResponse ShippingAddress { get; set; }
        public IEnumerable<OrderDetailResponse> OrderDetail { get; set; }

        public OrderResponse(string orderId, string customerId, string customerEmail, string customerName, int statusId, string comment,
            string paymentRef, OrderAddressResponse address, List<OrderDetailResponse> orderDetail)
        {
            Id = orderId;
            CustomerId = customerId;
            CustomerEmail = customerEmail;
            CustomerName = customerName;
            OrderStatusId = statusId;
            Comment = comment;
            PaymentRef = paymentRef;
            ShippingAddress = address;
            OrderDetail = orderDetail;
        }

        public OrderResponse(string orderId, string customerId, string customerEmail, int statusId, string orderStatusDesc,
            string comment, string paymentRef, DateTime date)
        {
            Id = orderId;
            CustomerId = customerId;
            CustomerEmail = customerEmail;
            OrderStatusDesc = orderStatusDesc;
            OrderStatusId = statusId;
            Comment = comment;
            PaymentRef = paymentRef;
            Date = date;
        }

        public OrderResponse() { }
    }
}
