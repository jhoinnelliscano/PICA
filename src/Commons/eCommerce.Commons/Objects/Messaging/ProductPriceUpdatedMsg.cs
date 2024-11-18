
namespace eCommerce.Commons.Objects.Messaging
{
    public class ProductPriceUpdatedMsg
    {
        public long ProductId { get; set; }
        public decimal NewPrice { get; set; }

        public ProductPriceUpdatedMsg(long productId, decimal newPrice) 
        {
            ProductId = productId;
            NewPrice = newPrice;
        }

        public ProductPriceUpdatedMsg() { }
    }
}
