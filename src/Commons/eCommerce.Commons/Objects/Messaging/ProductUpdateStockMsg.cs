
namespace eCommerce.Commons.Objects.Messaging
{
    public class ProductUpdateStockMsg
    {
        public long Id { get; set; }
        public bool Available { get; set; }

        public ProductUpdateStockMsg(long id, bool available) 
        {
            Id = id;
            Available = available;
        }

        public ProductUpdateStockMsg() { }
    }
}
