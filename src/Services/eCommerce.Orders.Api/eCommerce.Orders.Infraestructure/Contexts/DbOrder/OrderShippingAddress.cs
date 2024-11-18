using System;
using System.Collections.Generic;

namespace eCommerce.Orders.Infraestructure.Contexts.DbOrder
{
    public partial class OrderShippingAddress
    {
        public long Id { get; set; }
        public string OrderId { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string? AddressDesc { get; set; }
        public string? OtherInformation { get; set; }
        public string? City { get; set; }
        public string? Deparment { get; set; }
        public string? PostalCode { get; set; }

        public virtual Order Order { get; set; } = null!;
    }
}
