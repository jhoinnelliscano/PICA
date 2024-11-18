using System.ComponentModel.DataAnnotations;

namespace eCommerce.Commons.Objects.Requests.Customer
{
    public class CreateCustomerRequest : CustomerRequest
    {
        [Required]
        public override string IdentTypeId { get; set; } = null!;
        [Required]
        public override string Identification { get; set; } = null!;
        [Required]
        public override string FirstName { get; set; } = null!;
        [Required]
        public override string LastName { get; set; } = null!;

        public override string Email { get; set; } = null!;

        public override string Phone1 { get; set; } = null!;

        public override string AutenticationType { get; set; }

    }
}
