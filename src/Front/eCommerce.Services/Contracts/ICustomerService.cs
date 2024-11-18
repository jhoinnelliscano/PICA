using eCommerce.Commons.Objects.Requests.Customer;
using eCommerce.Commons.Objects.Responses;
using eCommerce.Commons.Objects.Responses.Customer;

namespace eCommerce.Services.Contracts
{
    public interface ICustomerService
    {
        Task<ServiceResponse<IEnumerable<CustomerIdentResponse>>> GetIdentificationsType();
        Task<ServiceResponse<CustomerResponse>> GetCustomer();
        Task<ServiceResponse<bool>> CreateCustomer(CreateCustomerRequest request);
        Task<ServiceResponse<bool>> UpdateCustomer(CustomerRequest request);
        Task<ServiceResponse<IEnumerable<CustomerAddressResponse>>> GetAddress();
        Task<ServiceResponse<bool>> CreateAddress(CustomerAddressRequest request);
        Task<ServiceResponse<bool>> UpdateAddress(CustomerAddressRequest request);
    }
}
