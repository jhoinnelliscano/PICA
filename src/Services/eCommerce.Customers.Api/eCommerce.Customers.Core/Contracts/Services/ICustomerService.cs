using eCommerce.Customers.Core.Objects.Dtos;

namespace eCommerce.Customers.Core.Contracts.Services
{
    public interface  ICustomerService
    {
        Task CreateCustomerAsync(CustomerDto customer);
        Task UpdateCustomerAsync(CustomerDto customer);
        Task<CustomerDto?> GetCustomerByIdAsync(string customerId);
        Task CreateCustomerAddressAsync(CustomerAddressDto customerEntity);
        Task UpdateCustomerAddressAsync(CustomerAddressDto customerEntity);
        Task InativeCustomerAddressAsync(long customerAddressId);
        IEnumerable<CustomerAddressDto?> GetCustomerAddressByCustomerId(string customerId);
        IEnumerable<IdentificationTypeDto> GetIdentificationsType();
    }
}
