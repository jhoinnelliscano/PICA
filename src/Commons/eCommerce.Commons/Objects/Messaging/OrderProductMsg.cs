
namespace eCommerce.Commons.Objects.Messaging
{
    public class OrderProductMsg
    {
        public long ProductId { get; set; }
        public int Units { get; set; }

        public OrderProductMsg(long productId, int units)
        {
            ProductId = productId;
            Units = units;
        }

        public OrderProductMsg() { }
    }
}
