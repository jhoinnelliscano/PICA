
namespace eCommerce.Commons.Objects.Requests.Order
{
    public class UpdateOrderStatusRequest
    {
        public string OrderId { get; set; }
        public int OrderStatusId { get; set; }
        public string? PaymentRef { get; set; }
        public PaymentRefRequest PaymentData { get; set; }
    }

}
