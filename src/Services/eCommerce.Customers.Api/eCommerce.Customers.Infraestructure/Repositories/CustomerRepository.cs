using eCommerce.Customers.Core.Contracts.Repositories;
using eCommerce.Customers.Core.Objects.DbTypes;
using eCommerce.Customers.Infraestructure.Contexts.DbCustomers;
using eCommerce.Customers.Infraestructure.Models.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.Customers.Infraestructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly DbCustomerContext _dbcontext;

        public CustomerRepository(DbCustomerContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task CreateCustomerAsync(CustomerEntity customerEntity)
        {
            var customer = new Customer() 
            {
                 Id = customerEntity.Id,
                 Identification = customerEntity.Identification,
                 IdentTypeId = customerEntity.IdentTypeId,
                 FirstName = customerEntity.FirstName,
                 SecondName = customerEntity.SecondName,
                 LastName = customerEntity.LastName,
                 SecondLastName = customerEntity.SecondLastName,
                 Email = customerEntity.Email,
                 Phone1 = customerEntity.Phone1,
                 Phone2 = customerEntity.Phone2,
                 UserName = customerEntity.Email,
                 AutenticationType = customerEntity.AutenticationType,
                 Status = "A",
                 CreatedAt = DateTime.Now
            };
            await _dbcontext.Customers.AddAsync(customer);
        }
       
        public async Task UpdateCustomerAsync(CustomerEntity customerEntity)
        {
            var customer = await _dbcontext.Customers.FindAsync(customerEntity.Id);

            if (customer != null)
            {
                customer.FirstName = !string.IsNullOrEmpty(customerEntity.FirstName) ? customerEntity.FirstName : customer.FirstName;
                customer.SecondName = !string.IsNullOrEmpty(customerEntity.SecondName) ? customerEntity.SecondName : customer.SecondName;
                customer.LastName = !string.IsNullOrEmpty(customerEntity.LastName) ? customerEntity.LastName : customer.LastName;
                customer.SecondLastName = !string.IsNullOrEmpty(customerEntity.SecondLastName) ? customerEntity.SecondLastName : customer.SecondLastName;
                customer.Identification = !string.IsNullOrEmpty(customerEntity.Identification) ? customerEntity.Identification : customer.Identification;
                customer.AutenticationType = !string.IsNullOrEmpty(customerEntity.AutenticationType) ? customerEntity.AutenticationType : customer.AutenticationType;
                customer.IdentTypeId = !string.IsNullOrEmpty(customerEntity.IdentTypeId) ? customerEntity.IdentTypeId : customer.IdentTypeId;
                customer.Email = !string.IsNullOrEmpty(customerEntity.Email) ? customerEntity.Email : customer.Email;
                customer.UserName = !string.IsNullOrEmpty(customerEntity.UserName) ? customerEntity.UserName : customer.UserName;
                customer.Status = !string.IsNullOrEmpty(customerEntity.Status) ? customerEntity.Status : customer.Status;
                customer.Phone1 = !string.IsNullOrEmpty(customerEntity.Phone1) ? customerEntity.Phone1 : customer.Phone1;
                customer.Phone2 = !string.IsNullOrEmpty(customerEntity.Phone2) ? customerEntity.Phone2 : customer.Phone2;
            }
        }
        
        public async Task<CustomerEntity?> GetCustomerByIdAsync(string customerId)
        {
            var customer = await _dbcontext.Customers.FindAsync(customerId);
            if (customer == null) return null;

            return new CustomerEntity(customer.Id, customer.IdentTypeId, customer.Identification, customer.FirstName, customer.SecondName,
                customer.LastName, customer.SecondName, customer.Email, customer.Phone1, customer.Phone2, customer.UserName, 
                customer.AutenticationType);
        }
  
        public async Task CreateCustomerAddressAsync(CustomerAddressEntity customerEntity) 
        {
            if (customerEntity.Default)
            {
                var address = _dbcontext.CustomerAddresses.Where(x => x.CustomertId.Equals(customerEntity.CustomertId)).ToList();
                foreach (var ad in address)
                {
                    ad.Default = false;
                }
            }

            var customerAddress = new CustomerAddress()
            {
                CustomertId = customerEntity.CustomertId,
                FirstName = customerEntity.FirstName, 
                LastName = customerEntity.LastName,
                Address = customerEntity.Address,
                AddressDesc = customerEntity.AddressDesc,
                OtherInformation = String.IsNullOrEmpty(customerEntity.OtherInformation) ? "N/A" : customerEntity.OtherInformation,
                City = customerEntity.City,
                Deparment = customerEntity.Deparment,
                PostalCode = customerEntity.PostalCode,
                Default = customerEntity.Default,
                CreatedAt = DateTime.Now,
                Status = true,
                Email = customerEntity.Email,
                Phone = customerEntity.Phone,
            };
            await _dbcontext.CustomerAddresses.AddAsync(customerAddress);
        }

        public async Task UpdateCustomerAddressAsync(CustomerAddressEntity customerEntity)
        {
            var customerAddress = await _dbcontext.CustomerAddresses.FindAsync(customerEntity.Id);

            if (customerAddress != null)
            {
                customerAddress.FirstName = !string.IsNullOrEmpty(customerEntity.FirstName) ? customerEntity.FirstName : customerAddress.FirstName;
                customerAddress.LastName = !string.IsNullOrEmpty(customerEntity.LastName) ? customerEntity.LastName : customerAddress.LastName;
                customerAddress.Address = !string.IsNullOrEmpty(customerEntity.Address) ? customerEntity.Address : customerAddress.Address;
                customerAddress.AddressDesc = !string.IsNullOrEmpty(customerEntity.AddressDesc) ? customerEntity.AddressDesc : customerAddress.AddressDesc;
                customerAddress.OtherInformation = !string.IsNullOrEmpty(customerEntity.OtherInformation) ? customerEntity.OtherInformation : customerAddress.OtherInformation;
                customerAddress.City = !string.IsNullOrEmpty(customerEntity.City) ? customerEntity.City : customerAddress.City;
                customerAddress.Deparment = !string.IsNullOrEmpty(customerEntity.Deparment) ? customerEntity.Deparment : customerAddress.Deparment;
                customerAddress.PostalCode = !string.IsNullOrEmpty(customerEntity.PostalCode) ? customerEntity.PostalCode : customerAddress.PostalCode;
                customerAddress.Email = !string.IsNullOrEmpty(customerEntity.Email) ? customerEntity.Email : customerAddress.Email;
                customerAddress.Phone = !string.IsNullOrEmpty(customerEntity.Phone) ? customerEntity.Phone : customerAddress.Phone;

                if (customerEntity.Default)
                    await SetCustomerAddressToDefaultAsync(customerAddress.CustomertId, customerAddress.Id);
                else
                    customerAddress.Default = false;
            }
        }

        public async Task InativeCustomerAddressAsync(long customerAddressId)
        {
            var customerAddress = await _dbcontext.CustomerAddresses.FindAsync(customerAddressId);

            if (customerAddress != null)
            {
                customerAddress.Status = false;
            }
        }

        public IEnumerable<CustomerAddressEntity>? GetCustomerAddressByCustomerId(string customerId)
        {
            var customerAddress =  _dbcontext.CustomerAddresses.Where(x => x.CustomertId.Equals(customerId) && x.Status);
            if (customerAddress == null) return null;

            return customerAddress.Select(x => new CustomerAddressEntity(x.Id, x.CustomertId, x.FirstName, x.LastName, x.Address, x.AddressDesc,
                x.OtherInformation, x.City, x.Deparment, x.PostalCode, x.Default, x.Email, x.Phone));
        }

        public IEnumerable<IdentificationTypeEntity> GetIdentificationsType() 
        {
            var result = _dbcontext.CustomerIdentTypes.OrderBy(x => x.Id);
            return result.Select(x => new IdentificationTypeEntity(x.Id, x.Name));
        }

        public async Task SetCustomerAddressToDefaultAsync(string customerId, long addressId)
        {
            var address = _dbcontext.CustomerAddresses.Where(x => x.CustomertId.Equals(customerId)).ToList();
            foreach (var ad in address)
            {
                ad.Default = false;
            }
            var addressDf = await _dbcontext.CustomerAddresses.FindAsync(addressId);

            if(addressDf != null)
                addressDf.Default = true;
        }
    }
}
