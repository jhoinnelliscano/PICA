
namespace eCommerce.Customers.Core.Objects.Dtos
{
    public class CustomerDto
    {
        public string Id { get; set; } = null!;
        public string IdentTypeId { get; set; } = null!;
        public string Identification { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string? SecondName { get; set; }
        public string LastName { get; set; } = null!;
        public string? SecondLastName { get; set; }
        public string Email { get; set; } = null!;
        public string Phone1 { get; set; } = null!;
        public string? Phone2 { get; set; }
        public string UserName { get; set; } = null!;
        public string AutenticationType { get; set; }
        public string Status { get; set; }

        public CustomerDto(string id, string identTypeId, string identification, string firstName, string? secondName,
            string lastName, string? secondLastName, string email, string phone1, string? phone2, string userName,
            string autenticationType)
        {
            Id = id;
            IdentTypeId = identTypeId;
            Identification = identification;
            FirstName = firstName;
            SecondName = secondName;
            LastName = lastName;
            SecondLastName = secondLastName;
            Email = email;
            Phone1 = phone1;
            Phone2 = phone2;
            UserName = userName;
            AutenticationType = autenticationType;
        }
    }
}
