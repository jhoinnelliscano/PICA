using eCommerce.Blazor.Demo.Services;
using eCommerce.Commons.Objects.Requests.Customer;
using eCommerce.Commons.Objects.Responses;
using eCommerce.Commons.Objects.Responses.Customer;
using eCommerce.Services.Contracts;
using Microsoft.Extensions.Configuration;

namespace eCommerce.Services.Services
{
    public class CustomerService : BaseHttpClient, ICustomerService
    {
        private const string Controller = "Customer";

        public CustomerService(HttpClient http, IConfiguration configuration) : base(http)
        {

        }

        public async Task<ServiceResponse<IEnumerable<CustomerIdentResponse>>> GetIdentificationsType()
        {
            try
            {
                const string Metodo = "IdentificationsType";
                string uri = SetUriQueryParameters(Controller, Metodo);
                return await GetAsync<ServiceResponse<IEnumerable<CustomerIdentResponse>>>(uri);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<ServiceResponse<CustomerResponse>> GetCustomer()
        {
            try
            {
                string Metodo = string.Empty;
                string uri = SetUriQueryParameters(Controller, Metodo);
                return await GetAsync<ServiceResponse<CustomerResponse>>(uri);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ServiceResponse<bool>> CreateCustomer(CreateCustomerRequest request)
        {
            try
            {
                string Metodo = string.Empty;
                string uri = SetUriQueryParameters(request, Controller, Metodo);    
                return await PostAsync<ServiceResponse<bool>>(uri, SetJsonContent(request));
            }
            catch
            {
                throw;
            }
        }

        public async Task<ServiceResponse<bool>> UpdateCustomer(CustomerRequest request)
        {
            try
            {
                string Metodo = string.Empty;
                string uri = SetUriQueryParameters(request, Controller, Metodo);
                return await PutAsync<ServiceResponse<bool>>(uri, SetJsonContent(request));
            }
            catch
            {
                throw;
            }
        }

        public async Task<ServiceResponse<IEnumerable<CustomerAddressResponse>>> GetAddress()
        {
            try
            {
                const string Metodo = "Address";
                string uri = SetUriQueryParameters(Controller, Metodo);
                return await GetAsync<ServiceResponse<IEnumerable<CustomerAddressResponse>>>(uri);
            }
            catch
            {
                throw;
            }
        }

        public async Task<ServiceResponse<bool>> CreateAddress(CustomerAddressRequest request)
        {
            try
            {
                const string Metodo = "Address";
                string uri = SetUriQueryParameters(request, Controller, Metodo);
                return await PostAsync<ServiceResponse<bool>>(uri, SetJsonContent(request));
            }
            catch
            {
                throw;
            }
        }

        public async Task<ServiceResponse<bool>> UpdateAddress(CustomerAddressRequest request)
        {
            try
            {
                const string Metodo = "Address";
                string uri = SetUriQueryParameters(request, Controller, Metodo);
                return await PutAsync<ServiceResponse<bool>>(uri, SetJsonContent(request));
            }
            catch
            {
                throw;
            }
        }
    }
}
