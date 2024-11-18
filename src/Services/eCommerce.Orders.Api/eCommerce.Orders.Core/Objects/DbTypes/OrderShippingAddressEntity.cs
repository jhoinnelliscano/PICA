
namespace eCommerce.Orders.Core.Objects.DbTypes
{
    public class OrderShippingAddressEntity
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

        public OrderShippingAddressEntity(long id, string orderId, string firstName, string lastName, string email,
            string phone, string address, string addressDesc, string otherInfo, string city, string deparment, string postalCode)
        {
            Id = id;
            OrderId = orderId;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Phone = phone;
            Address = address;
            AddressDesc = addressDesc;
            OtherInformation = otherInfo;
            City = city;
            Deparment = deparment;
            PostalCode = postalCode;
        }

        public OrderShippingAddressEntity() { }
    }
}
