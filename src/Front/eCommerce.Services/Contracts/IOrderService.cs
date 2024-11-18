using eCommerce.Commons.Objects.Requests.Order;
using eCommerce.Commons.Objects.Responses;
using eCommerce.Commons.Objects.Responses.Order;

namespace eCommerce.Services.Contracts
{
    public interface IOrderService
    {
        Task<ServiceResponse<CreateOrderResponse>> CreateOrder(CreateOrderRequest request);
        Task<ServiceResponse<IEnumerable<OrderResponse>>> GetOrders(GetOrderByUserRequest request);
        Task<ServiceResponse<OrderResponse>> GetOrder(GetOrderRequest request);
        Task<ServiceResponse<bool>> UpdateOrderStatus(UpdateOrderStatusRequest request);
    }
}
