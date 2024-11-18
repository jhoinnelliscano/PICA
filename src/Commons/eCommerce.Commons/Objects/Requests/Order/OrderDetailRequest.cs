

namespace eCommerce.Commons.Objects.Requests.Order
{
    public class OrderDetailRequest
    {
        public long ProductId { get; set; }
        public long ShoppingCartId { get; set; }
        public string ProductName { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal Price { get; set; }


        public OrderDetailRequest(long productId, long shoppingCartId, string productName,
            int quantity, decimal price)
        {
            ProductId = productId;
            ShoppingCartId = shoppingCartId;
            ProductName = productName;
            Quantity = quantity;
            Price = price;
        }

        public OrderDetailRequest() { }
    }
}
