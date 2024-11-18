
namespace eCommerce.Commons.Objects.Responses.Customer
{
    public class CustomerIdentResponse
    {
        public string Id { get; set; }
        public string Description { get; set; }

        public CustomerIdentResponse(string id, string description) 
        {
            Id = id;
            Description = description;
        }
    }
}
