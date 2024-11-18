using eCommerce.Blazor.Demo.Services;
using eCommerce.Commons.Objects.Requests.Products;
using eCommerce.Commons.Objects.Responses;
using eCommerce.Services.Contracts;

namespace eCommerce.Services.Services
{
    public class ProductManagerService : BaseHttpClient, IProductManagerService
    {
        private const string Controller = "Product";

        public ProductManagerService(HttpClient http) : base(http)
        {

        }

        public async Task<ServiceResponse<bool>> UpdateProduct(UpdateProductRequest request)
        {
            try
            {
                string uri = SetUriQueryParameters(request, Controller, string.Empty);
                return await PostAsync<ServiceResponse<bool>>(uri, SetJsonContent(request));
            }
            catch
            {
                throw;
            }
        }
    }
}
