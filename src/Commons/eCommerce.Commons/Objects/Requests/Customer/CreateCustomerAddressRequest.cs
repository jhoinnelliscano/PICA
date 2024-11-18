
using System.ComponentModel.DataAnnotations;

namespace eCommerce.Commons.Objects.Requests.Customer
{
    public class CreateCustomerAddressRequest : CustomerAddressRequest
    {
        [Required]
        public override string FirstName { get; set; } = null!;
        [Required]
        public override string LastName { get; set; } = null!;
        [Required]
        public override string Address { get; set; } = null!;
        [Required]
        public override string AddressDesc { get; set; }
        [Required]
        public override string Email { get; set; }
        [Required]
        public override string Phone { get; set; }
    }
}
