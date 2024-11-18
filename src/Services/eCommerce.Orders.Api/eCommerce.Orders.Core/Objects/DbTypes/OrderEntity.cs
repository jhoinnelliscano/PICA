using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.Orders.Core.Objects.DbTypes
{
    public class OrderEntity
    {
        public string Id { get; set; } = null!;
        public string CustomerId { get; set; } = null!;
        public string CustomerEmail { get; set; } = null!;
        public int OrderStatusId { get; set; }
        public string OrderStatusDesc { get; set; }
        public string? Comment { get; set; }
        public string? PaymentRef { get; set; }
        public decimal Total { get; set; }
        public DateTime Date { get; set; }
        public OrderShippingAddressEntity ShippingAddress { get; set; }
        public IEnumerable<OrderDetailEntity> Detail { get; set; }
        public OrderPaymentEntity PaymentData { get; set; }


        public OrderEntity(string orderId, string customerId, string customerEmail, int statusId, string comment,
            string paymentRef, OrderShippingAddressEntity address, List<OrderDetailEntity> orderDetail)
        {
            Id = orderId;
            CustomerId = customerId;
            CustomerEmail = customerEmail;
            OrderStatusId = statusId;
            Comment = comment;
            PaymentRef = paymentRef;
            ShippingAddress = address;
            Detail = orderDetail;
        }

        public OrderEntity() { }
    }
}
