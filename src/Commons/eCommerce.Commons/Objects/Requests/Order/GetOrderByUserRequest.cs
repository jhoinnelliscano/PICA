

namespace eCommerce.Commons.Objects.Requests.Order
{
    public class GetOrderByUserRequest
    {
        public string CustomerId { get; set; }
        public int OrderStatusId { get; set; }
    }
}
