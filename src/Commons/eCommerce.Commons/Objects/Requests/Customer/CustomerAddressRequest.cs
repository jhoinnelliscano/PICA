
using System.ComponentModel.DataAnnotations;

namespace eCommerce.Commons.Objects.Requests.Customer
{
    public class CustomerAddressRequest
    {
        [Required]
        public long Id { get; set; }
        public string? CustomertId { get; set; } = null!;
        public virtual string? FirstName { get; set; } = null!;
        public virtual string? LastName { get; set; } = null!;
        public virtual string? Address { get; set; } = null!;
        public virtual string? AddressDesc { get; set; }
        public virtual string? OtherInformation { get; set; } = null!;
        public virtual string? City { get; set; }
        public virtual string? Deparment { get; set; }
        public virtual string? PostalCode { get; set; }
        public virtual string? Email { get; set; }
        public virtual string? Phone { get; set; }
        public bool Default { get; set; }
        public bool Inactive { get; set; }
    }
}
