using System;
using System.Collections.Generic;

namespace eCommerce.Orders.Infraestructure.Contexts.DbOrder
{
    public partial class OrderDetail
    {
        public long Id { get; set; }
        public string OrderId { get; set; } = null!;
        public long ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string? ProductDescription { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public long? IdShoppingCart { get; set; }

        public virtual Order Order { get; set; } = null!;
    }
}
