
namespace eCommerce.Customers.Core.Objects.Dtos
{
    public class IdentificationTypeDto
    {
        public string Id { get; set; }
        public string Description { get; set; }

        public IdentificationTypeDto(string id, string description)
        {
            Id = id;
            Description = description;
        }
    }
}
