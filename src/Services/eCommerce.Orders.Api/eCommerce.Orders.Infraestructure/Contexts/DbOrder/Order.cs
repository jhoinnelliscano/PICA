using System;
using System.Collections.Generic;

namespace eCommerce.Orders.Infraestructure.Contexts.DbOrder
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
            OrderPayments = new HashSet<OrderPayment>();
            OrderShippingAddresses = new HashSet<OrderShippingAddress>();
        }

        public string Id { get; set; } = null!;
        public string CustomerId { get; set; } = null!;
        public string CustomerEmail { get; set; } = null!;
        public int OrderStatusId { get; set; }
        public string? Comment { get; set; }
        public string? PaymentRef { get; set; }
        public decimal Total { get; set; }
        public DateTime Date { get; set; }

        public virtual OrderStatus OrderStatus { get; set; } = null!;
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<OrderPayment> OrderPayments { get; set; }
        public virtual ICollection<OrderShippingAddress> OrderShippingAddresses { get; set; }
    }
}
