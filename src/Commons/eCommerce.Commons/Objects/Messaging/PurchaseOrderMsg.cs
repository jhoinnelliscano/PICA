
namespace eCommerce.Commons.Objects.Messaging
{
    public class PurchaseOrderMsg
    {
        public string UserId { get; set; }
        public IEnumerable<ProductMsg> Products { get; set; }
    }
}
