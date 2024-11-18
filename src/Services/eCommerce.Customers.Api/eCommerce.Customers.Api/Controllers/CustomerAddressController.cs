using eCommerce.Commons.Objects.Requests.Customer;
using eCommerce.Commons.Objects.Responses;
using eCommerce.Commons.Objects.Responses.Customer;
using eCommerce.Commons.Utilities;
using eCommerce.Customers.Core.Config;
using eCommerce.Customers.Core.Contracts.Services;
using eCommerce.Customers.Core.Helpers.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.Customers.Api.Controllers
{
    [Route("api/Customer/Address")]
    [ApiController]
    public class CustomerAddressController : ControllerBase
    {
        private readonly string _userClaim = AppConfiguration.Configuration["AppConfiguration:Auth0:Claims:UserClaim"].ToString();
        private readonly ILogger<CustomerController> _logger;
        private ICustomerService _customerService;
        private IMapperHelper _mapperHelper;

        public CustomerAddressController(ICustomerService customerService, ILogger<CustomerController> logger, IMapperHelper mapperHelper)
        {
            _customerService = customerService;
            _logger = logger;
            _mapperHelper = mapperHelper;
        }

        /// <summary>
        /// Obtine la lista de direccciones de un cliente
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public ActionResult<ServiceResponse<IEnumerable<CustomerAddressResponse>>> GetCustomerAddress()
        {
            var userId = UserUtilities.GetUserId(User.Claims.FirstOrDefault(c => c.Type == _userClaim).Value);

            var result = _customerService.GetCustomerAddressByCustomerId(userId);
            if(result == null) return Ok(new ServiceResponse<IEnumerable<CustomerAddressResponse>>("Successful", null));

            var response = result.Select(x => _mapperHelper.MappToCustomerAddressResponse(x));
            return Ok(new ServiceResponse<IEnumerable<CustomerAddressResponse>>("Successful", response));
        }

        /// <summary>
        /// Crea una nueva dirección asociada a un cliente
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ServiceResponse<bool>>> Create([FromBody] CreateCustomerAddressRequest request)
        {
            var userId = UserUtilities.GetUserId(User.Claims.FirstOrDefault(c => c.Type == _userClaim).Value);

            request.CustomertId = userId;
            var address = _mapperHelper.MappToCustomerAddressDto(request);

            await _customerService.CreateCustomerAddressAsync(address);
            return Ok(new ServiceResponse<bool>("Successful", true));
        }

        /// <summary>
        /// Actualzia una dirección
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize]
        public async Task<ActionResult<ServiceResponse<bool>>> Update([FromBody] CustomerAddressRequest request)
        {
            var userId = UserUtilities.GetUserId(User.Claims.FirstOrDefault(c => c.Type == _userClaim).Value);
            request.CustomertId = userId;

            if (request.Inactive) 
            {
                await _customerService.InativeCustomerAddressAsync(request.Id);
            }
            else
            {
                var address = _mapperHelper.MappToCustomerAddressDto(request);
                await _customerService.UpdateCustomerAddressAsync(address);
            }
            return Ok(new ServiceResponse<bool>("Successful", true));
        }

    }
}
