using eCommerce.Blazor.Demo.Services;
using eCommerce.Commons.Objects.Requests.Order;
using eCommerce.Commons.Objects.Responses;
using eCommerce.Commons.Objects.Responses.Order;
using eCommerce.Services.Contracts;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.Services.Services
{
    public class OrderService : BaseHttpClient, IOrderService
    {
        private const string Controller = "Order";
        public OrderService(HttpClient http, IConfiguration configuration) : base(http)
        {

        }

        public async Task<ServiceResponse<CreateOrderResponse>> CreateOrder(CreateOrderRequest request)
        {
            try
            {
                string uri = GetUri(Controller);
                return await PostAsync<ServiceResponse<CreateOrderResponse>>(uri, SetJsonContent(request));
            }
            catch
            {
                throw;
            }
        }

        public async Task<ServiceResponse<IEnumerable<OrderResponse>>> GetOrders(GetOrderByUserRequest request)
        {
            try
            {
                string uri = SetUriQueryParameters(request, Controller, string.Empty);
                return await GetAsync<ServiceResponse<IEnumerable<OrderResponse>>>(uri);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ServiceResponse<OrderResponse>> GetOrder(GetOrderRequest request)
        {

            string uri = SetUriQueryParameters(request, Controller, "Detail");
            return await GetAsync<ServiceResponse<OrderResponse>>(uri);
        }

        public async Task<ServiceResponse<bool>> UpdateOrderStatus(UpdateOrderStatusRequest request)
        {
            try
            {
                string uri = GetUri(Controller);
                return await PutAsync<ServiceResponse<bool>>(uri, SetJsonContent(request));
            }
            catch
            {
                throw;
            }
        }
    }
}
