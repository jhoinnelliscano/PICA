
using eCommerce.Commons.Objects.Requests.Products;
using eCommerce.Commons.Objects.Responses;

namespace eCommerce.Services.Contracts
{
    public interface IProductManagerService
    {
        Task<ServiceResponse<bool>> UpdateProduct(UpdateProductRequest request);
    }
}
