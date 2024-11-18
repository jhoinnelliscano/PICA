using eCommerce.Commons.Objects.Requests.Customer;
using eCommerce.Commons.Objects.Responses.Customer;
using eCommerce.Customers.Core.Objects.Dtos;


namespace eCommerce.Customers.Core.Helpers.Mappers
{
    public class MapperHelper : IMapperHelper
    {
        public CustomerDto MappToCustomerDto(CreateCustomerRequest request, string userId)
        { 
          return  new CustomerDto(userId, request.IdentTypeId, request.Identification, request.FirstName, request.SecondName,
                request.LastName, request.SecondName, request.Email, request.Phone1, request.Phone2, request.Email, request.AutenticationType);
        }

        public CustomerDto MappToCustomerDto(CustomerRequest request, string userId)
        {
            return new CustomerDto(userId, request.IdentTypeId, request.Identification, request.FirstName, request.SecondName,
                  request.LastName, request.SecondName, request.Email, request.Phone1, request.Phone2, request.Email, request.AutenticationType);
        }

        public CustomerAddressDto MappToCustomerAddressDto(CreateCustomerAddressRequest request, string userId)
        {
            return new CustomerAddressDto(0, userId, request.FirstName, request.LastName, request.Address,
                request.AddressDesc, request.OtherInformation, request.City, request.Deparment, request.PostalCode,
                request.Default, request.Email, request.Phone);
        }

        public CustomerResponse MappToCustomerResponse(CustomerDto response) 
        {
            return new CustomerResponse(response.Id, response.IdentTypeId, response.Identification, response.FirstName, response.SecondName,
                response.LastName, response.SecondName, response.Email, response.Phone1, response.Phone2, response.UserName, response.AutenticationType);
        }

        public CustomerAddressResponse MappToCustomerAddressResponse(CustomerAddressDto response) 
        {
            return new CustomerAddressResponse(response.Id, response.CustomertId, response.FirstName, response.LastName,
                response.Address, response.AddressDesc, response.OtherInformation, response.City,
                response.Deparment, response.PostalCode, response.Default, response.Email, response.Phone);
        }

        public CustomerAddressDto MappToCustomerAddressDto(CustomerAddressRequest request) 
        {
            return new CustomerAddressDto(request.Id, request.CustomertId, request.FirstName, request.LastName,
                request.Address, request.AddressDesc, request.OtherInformation, request.City,
                request.Deparment, request.PostalCode, request.Default, request.Email, request.Phone);
        }

        public CustomerAddressDto MappToCustomerAddressDto(CreateCustomerAddressRequest request)
        {
            return new CustomerAddressDto(request.Id, request.CustomertId, request.FirstName, request.LastName,
                request.Address, request.AddressDesc, request.OtherInformation, request.City,
                request.Deparment, request.PostalCode, request.Default, request.Email, request.Phone);
        }
    }
}
