
namespace eCommerce.Orders.Core.Objects.Dtos
{
    public class OrderDto
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
        public OrderShippingAddressDto ShippingAddress { get; set; }
        public IEnumerable<OrderDetailDto> OrderDetail { get; set; }
        public OrderPaymentDto PaymentData { get; set; }

        public OrderDto(string orderId, string customerId, string customerEmail, string customerName, int statusId,  string comment, 
            string paymentRef, OrderShippingAddressDto address,List<OrderDetailDto> orderDetail)
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

        public OrderDto(string orderId, string customerId, string customerEmail, int statusId, string orderStatusDesc, 
            string comment, string paymentRef, DateTime date, decimal total)
        {
            Id = orderId;
            CustomerId = customerId;
            CustomerEmail = customerEmail;
            OrderStatusDesc = orderStatusDesc;
            OrderStatusId = statusId;
            Comment = comment;
            PaymentRef = paymentRef;
            Date = date;
            Total = total;
        }

        public OrderDto() { }
    }
}
