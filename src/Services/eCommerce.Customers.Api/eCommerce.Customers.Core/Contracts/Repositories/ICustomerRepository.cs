using eCommerce.Customers.Core.Objects.DbTypes;

namespace eCommerce.Customers.Core.Contracts.Repositories
{
    public interface ICustomerRepository
    {
        Task CreateCustomerAsync(CustomerEntity customerEntity);
        Task UpdateCustomerAsync(CustomerEntity customerEntity);
        Task<CustomerEntity?> GetCustomerByIdAsync(string customerId);
        Task CreateCustomerAddressAsync(CustomerAddressEntity customerEntity);
        Task UpdateCustomerAddressAsync(CustomerAddressEntity customerEntity);
        Task InativeCustomerAddressAsync(long customerAddressId);
        IEnumerable<CustomerAddressEntity>? GetCustomerAddressByCustomerId(string customerId);
        IEnumerable<IdentificationTypeEntity> GetIdentificationsType();
        Task SetCustomerAddressToDefaultAsync(string customerId, long addressId);
    }
}
