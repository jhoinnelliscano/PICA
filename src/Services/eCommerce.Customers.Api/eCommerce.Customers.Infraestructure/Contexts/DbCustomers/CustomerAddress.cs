using System;
using System.Collections.Generic;

namespace eCommerce.Customers.Infraestructure.Contexts.DbCustomers
{
    public partial class CustomerAddress
    {
        public long Id { get; set; }
        public string CustomertId { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string? AddressDesc { get; set; }
        public string OtherInformation { get; set; } = null!;
        public string? City { get; set; }
        public string? Deparment { get; set; }
        public string? PostalCode { get; set; }
        public bool Default { get; set; }
        public bool Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Customer Customert { get; set; } = null!;
    }
}
