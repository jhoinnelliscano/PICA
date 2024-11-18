using eCommerce.Commons.Objects.Requests.Customer;
using eCommerce.Commons.Objects.Responses.Customer;
using eCommerce.Customers.Core.Objects.Dtos;

namespace eCommerce.Customers.Core.Helpers.Mappers
{
    public interface IMapperHelper
    {
        CustomerDto MappToCustomerDto(CreateCustomerRequest request, string userId);
        CustomerDto MappToCustomerDto(CustomerRequest request, string userId);
        CustomerAddressDto MappToCustomerAddressDto(CreateCustomerAddressRequest request, string userId);
        CustomerResponse MappToCustomerResponse(CustomerDto response);
        CustomerAddressResponse MappToCustomerAddressResponse(CustomerAddressDto customerAddress);
        CustomerAddressDto MappToCustomerAddressDto(CustomerAddressRequest request);
        CustomerAddressDto MappToCustomerAddressDto(CreateCustomerAddressRequest request);
    }
}
