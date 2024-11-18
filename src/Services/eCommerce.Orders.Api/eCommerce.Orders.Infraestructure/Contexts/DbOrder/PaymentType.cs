﻿using System;
using System.Collections.Generic;

namespace eCommerce.Orders.Infraestructure.Contexts.DbOrder
{
    public partial class PaymentType
    {
        public PaymentType()
        {
            OrderPayments = new HashSet<OrderPayment>();
        }

        public string Id { get; set; } = null!;
        public string? Description { get; set; }

        public virtual ICollection<OrderPayment> OrderPayments { get; set; }
    }
}
