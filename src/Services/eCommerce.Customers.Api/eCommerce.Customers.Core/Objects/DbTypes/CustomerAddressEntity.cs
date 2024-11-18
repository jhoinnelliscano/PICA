
namespace eCommerce.Customers.Core.Objects.DbTypes
{
    public class CustomerAddressEntity
    {
        public long Id { get; set; }
        public string CustomertId { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string? AddressDesc { get; set; }
        public string OtherInformation { get; set; } = null!;
        public string? City { get; set; }
        public string? Deparment { get; set; }
        public string? PostalCode { get; set; }
        public bool Default { get; set; }

        public CustomerAddressEntity(long id, string customertId, string firstName, string lastName, string address, string? addressDesc,
            string otherInformation, string? city, string? deparment, string? postalCode, bool isDefault, string email, string phone) 
        {
            Id = id;
            CustomertId = customertId; ;
            FirstName = firstName;
            LastName = lastName;
            Address = address;
            AddressDesc = addressDesc;
            OtherInformation = otherInformation;
            City = city;
            Deparment = deparment;
            PostalCode = postalCode;
            Default = isDefault;
            Phone = phone;
            Email = email;
        }
    }
}
