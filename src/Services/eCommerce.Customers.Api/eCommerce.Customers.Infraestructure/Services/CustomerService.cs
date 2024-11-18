using eCommerce.Customers.Core.Contracts.Repositories;
using eCommerce.Customers.Core.Contracts.Services;
using eCommerce.Customers.Core.Objects.DbTypes;
using eCommerce.Customers.Core.Objects.Dtos;
using eCommerce.Customers.Infraestructure.Models.UnitOfWorks;

namespace eCommerce.Customers.Infraestructure.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CustomerService(ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
        {
            _customerRepository = customerRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task CreateCustomerAsync(CustomerDto customer)
        {
            var customerEntity = new CustomerEntity(customer.Id, customer.IdentTypeId, customer.Identification, customer.FirstName, customer.SecondName,
                customer.LastName, customer.SecondName, customer.Email, customer.Phone1, customer.Phone2, customer.UserName,
                customer.AutenticationType);

            await _customerRepository.CreateCustomerAsync(customerEntity);
            await _unitOfWork.ConfirmAsync();
        }

        public async Task UpdateCustomerAsync(CustomerDto customer)
        {
            var customerEntity = new CustomerEntity(customer.Id, customer.IdentTypeId, customer.Identification, customer.FirstName, customer.SecondName,
                customer.LastName, customer.SecondName, customer.Email, customer.Phone1, customer.Phone2, customer.UserName,
                customer.AutenticationType);
            customerEntity.Status = customer.Status;

            await _customerRepository.UpdateCustomerAsync(customerEntity);
            await _unitOfWork.ConfirmAsync();
        }

        public async Task<CustomerDto?> GetCustomerByIdAsync(string customerId)
        {
            var customer = await _customerRepository.GetCustomerByIdAsync(customerId);
            if (customer == null) return null;

            return new CustomerDto(customer.Id, customer.IdentTypeId, customer.Identification, customer.FirstName, customer.SecondName,
                customer.LastName, customer.SecondName, customer.Email, customer.Phone1, customer.Phone2, customer.UserName,
                customer.AutenticationType);
        }

        public async Task CreateCustomerAddressAsync(CustomerAddressDto customerAddress)
        {
            var customerAddressEntity = new CustomerAddressEntity(customerAddress.Id, customerAddress.CustomertId, customerAddress.FirstName, customerAddress.LastName,
                customerAddress.Address, customerAddress.AddressDesc, customerAddress.OtherInformation, customerAddress.City,
                customerAddress.Deparment, customerAddress.PostalCode, customerAddress.Default, customerAddress.Email, customerAddress.Phone);
            
            await _customerRepository.CreateCustomerAddressAsync(customerAddressEntity);
            await _unitOfWork.ConfirmAsync();
        }

        public async Task UpdateCustomerAddressAsync(CustomerAddressDto customerAddress)
        {
            var customerAddressEntity = new CustomerAddressEntity(customerAddress.Id, customerAddress.CustomertId, customerAddress.FirstName, customerAddress.LastName,
                customerAddress.Address, customerAddress.AddressDesc, customerAddress.OtherInformation, customerAddress.City,
                customerAddress.Deparment, customerAddress.PostalCode, customerAddress.Default, customerAddress.Email, customerAddress.Phone);

            await _customerRepository.UpdateCustomerAddressAsync(customerAddressEntity);

            if(customerAddress.Default)
                await _customerRepository.SetCustomerAddressToDefaultAsync(customerAddress.CustomertId, customerAddress.Id);

            await _unitOfWork.ConfirmAsync();
        }

        public async Task InativeCustomerAddressAsync(long customerAddressId)
        {
            await _customerRepository.InativeCustomerAddressAsync(customerAddressId);
            await _unitOfWork.ConfirmAsync();
        }

        public IEnumerable<CustomerAddressDto?> GetCustomerAddressByCustomerId(string customerId)
        {
            var customerAddress = _customerRepository.GetCustomerAddressByCustomerId(customerId);
            if (customerAddress == null) return null;

            return customerAddress.Select(x => new CustomerAddressDto(x.Id, x.CustomertId, x.FirstName, x.LastName, x.Address, x.AddressDesc,
                x.OtherInformation, x.City, x.Deparment, x.PostalCode, x.Default, x.Email, x.Phone));
        }

        public IEnumerable<IdentificationTypeDto> GetIdentificationsType()
        {
            var result = _customerRepository.GetIdentificationsType();
            return result.Select(x => new IdentificationTypeDto(x.Id, x.Description));
        }

    }
}
