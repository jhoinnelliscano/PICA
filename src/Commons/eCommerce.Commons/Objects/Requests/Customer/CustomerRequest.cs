
namespace eCommerce.Commons.Objects.Requests.Customer
{
    public class CustomerRequest
    {
        public virtual string IdentTypeId { get; set; } = null!;
        public virtual string Identification { get; set; } = null!;
        public virtual string FirstName { get; set; } = null!;
        public string? SecondName { get; set; }
        public virtual string LastName { get; set; } = null!;
        public string? SecondLastName { get; set; }
        public virtual string Email { get; set; } = null!;
        public virtual string? Phone1 { get; set; } = null!;
        public string? Phone2 { get; set; }
        public virtual string AutenticationType { get; set; }
        public string? Status { get; set; }
    }
}
