using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.Orders.Core.Objects.DbTypes
{
    public class OrderDetailEntity
    {
        public long Id { get; set; }
        public string OrderId { get; set; } = null!;
        public long ProductId { get; set; }
        public long ShoppingCartId { get; set; }
        public string ProductName { get; set; } = null!;
        public string? ProductDescription { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public decimal TotalByItem 
        {
            get { return Price * Quantity;  }
        }

        public OrderDetailEntity(long id, string orderId, long productId, long shoppingCartId, string productName, string productDescription,
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

        public OrderDetailEntity() { }
    }
}
