using System;
using System.Collections.Generic;

namespace eCommerce.Orders.Infraestructure.Contexts.DbOrder
{
    public partial class OrderPayment
    {
        public long Id { get; set; }
        public string OrderId { get; set; } = null!;
        public string? RefPayco { get; set; }
        public string? CustIdCliente { get; set; }
        public string? TransactionId { get; set; }
        public string? TransactionDate { get; set; }
        public string? ResponseCode { get; set; }
        public string? ResponseReason { get; set; }
        public string? PaymentFranchiseId { get; set; }
        public string? CustomerIp { get; set; }
        public string? PaymentTypeId { get; set; }
        public string? Amount { get; set; }
        public DateTime? CreatedAt { get; set; }

        public virtual Order Order { get; set; } = null!;
        public virtual PaymentFranchise? PaymentFranchise { get; set; }
        public virtual PaymentType? PaymentType { get; set; }
    }
}
