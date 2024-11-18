
namespace eCommerce.Commons.Objects.Messaging
{
    public class ProductMsg
    {
        public long Id { get; set; }
        public decimal Price { get; set; }
        public decimal NewPrice { get; set; }
        public int Units { get; set; }
        public bool ChangePrice { get; set; }
        public bool ChangeUnits { get; set; }

        public ProductMsg(long id, decimal price, decimal newPrice, int units, bool changePrice, bool changeUnits) 
        {
            Id = id;
            Price = price;
            NewPrice = newPrice;
            Units = units;
            ChangePrice = changePrice;
            ChangeUnits = changeUnits;
        }

        public ProductMsg(long id)
        {
            Id = id;
        }

        public ProductMsg() { }
    }
}
