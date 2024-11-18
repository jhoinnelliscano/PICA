using eCommerce.Orders.Core.Objects.Dtos;

namespace eCommerce.Orders.Core.Contracts.Services
{
    public interface IOrderService
    {
        Task<string> Create(OrderDto order);
        Task<IEnumerable<OrderDto>?> GetOrdersByCustomer(string customerId, int orderStatus);
        Task<OrderDto?> GetOrderByIdAsync(string orderId);
        Task UpdateOrderStatusAsync(OrderDto order);
    }
}
