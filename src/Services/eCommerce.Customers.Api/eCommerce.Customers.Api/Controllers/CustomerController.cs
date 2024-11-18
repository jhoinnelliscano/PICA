using eCommerce.Commons.Objects.Requests.Customer;
using eCommerce.Commons.Objects.Responses;
using eCommerce.Commons.Objects.Responses.Customer;
using eCommerce.Commons.Utilities;
using eCommerce.Customers.Core.Config;
using eCommerce.Customers.Core.Contracts.Services;
using eCommerce.Customers.Core.Helpers.Mappers;
using eCommerce.Customers.Core.Objects.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
namespace eCommerce.Customers.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly string _userClaim = AppConfiguration.Configuration["AppConfiguration:Auth0:Claims:UserClaim"].ToString();
        private ICustomerService _customerService;
        private IMapperHelper _mapperHelper;

        public CustomerController(ICustomerService customerService, IMapperHelper mapperHelper)
        {
            _customerService = customerService;
            _mapperHelper = mapperHelper;
        }


        /// <summary>
        /// Obtine tipos de identificaciones para clientes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("IdentificationsType")]
        [Authorize]
        public ActionResult<ServiceResponse<IEnumerable<CustomerIdentResponse>>> GetCustomerIdentificationType()
        {
            var result = _customerService.GetIdentificationsType();
            var response = result.Select(x => new CustomerIdentResponse(x.Id, x.Description));
            return Ok(new ServiceResponse<IEnumerable<CustomerIdentResponse>>("Successful", response));
        }

        /// <summary>
        /// Obtiene un cliente
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ServiceResponse<CustomerResponse>>> GetCustomer()
        {
            var userId = UserUtilities.GetUserId(User.Claims.FirstOrDefault(c => c.Type == _userClaim).Value);

            var result = await _customerService.GetCustomerByIdAsync(userId);
            if (result == null)
            {
                var responseNotFound = new CustomerResponse()
                {
                    Id = userId,
                    Email = User.Claims.FirstOrDefault(c => c.Type == "https://ecommerce.com/email").Value
                };
                return Ok(new ServiceResponse<CustomerResponse>("Successful", responseNotFound));
            }

            var response = _mapperHelper.MappToCustomerResponse(result);
            response.RegCompleted = true;
            return Ok(new ServiceResponse<CustomerResponse>("Successful", response));
        }

        /// <summary>
        /// Crea un nuevo cliente
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ServiceResponse<bool>>> Create([FromBody] CreateCustomerRequest request)
        {
            var auth0Id = User.Claims.FirstOrDefault(c => c.Type == _userClaim).Value;
            var userId = UserUtilities.GetUserId(auth0Id);
            request.AutenticationType = UserUtilities.GetUserAuthenticationType(auth0Id);

            var customer = _mapperHelper.MappToCustomerDto(request, userId);

            await _customerService.CreateCustomerAsync(customer);
            return Ok(new ServiceResponse<bool>("Successful", true));
        }
        
        /// <summary>
        /// Actualzia un cliente
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize]
        public async Task<ActionResult<ServiceResponse<bool>>> Update([FromBody] CustomerRequest request)
        {
            var userId = UserUtilities.GetUserId(User.Claims.FirstOrDefault(c => c.Type == _userClaim).Value);

            var customer = _mapperHelper.MappToCustomerDto(request, userId);
            await _customerService.UpdateCustomerAsync(customer);
            return Ok(new ServiceResponse<bool>("Successful", true));
        }
    }
}
