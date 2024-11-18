
namespace eCommerce.Customers.Core.Objects.DbTypes
{
    public class IdentificationTypeEntity
    {
        public string Id { get; set; }
        public string Description { get; set; }

        public IdentificationTypeEntity(string id, string description) 
        {
            Id = id;
            Description = description;
        }

        public IdentificationTypeEntity() { }
    }
}
