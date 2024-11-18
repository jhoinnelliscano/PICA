
namespace eCommerce.Commons.Objects.Responses.Order
{
    public class OrderDetailResponse
    {
        public long Id { get; set; }
        public string OrderId { get; set; } = null!;
        public long ProductId { get; set; }
        public long ShoppingCartId { get; set; }
        public string ProductName { get; set; } = null!;
        public string? ProductDescription { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }


        public OrderDetailResponse(long id, string orderId, long productId, long shoppingCartId, string productName, string productDescription,
            int quantity, decimal price)
        {
            Id = id;
            OrderId = orderId;
            ProductId = productId;
            ShoppingCartId = shoppingCartId;
            ProductName = productName;
            ProductDescription = productDescription;
            Quantity = quantity;
            Price = price;
        }

        public OrderDetailResponse() { }
    }
}
