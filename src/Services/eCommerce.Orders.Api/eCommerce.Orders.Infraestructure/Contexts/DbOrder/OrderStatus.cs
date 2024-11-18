using System;
using System.Collections.Generic;

namespace eCommerce.Orders.Infraestructure.Contexts.DbOrder
{
    public partial class OrderStatus
    {
        public OrderStatus()
        {
            Orders = new HashSet<Order>();
        }

        public int Id { get; set; }
        public string Status { get; set; } = null!;
        public string? Descripction { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
