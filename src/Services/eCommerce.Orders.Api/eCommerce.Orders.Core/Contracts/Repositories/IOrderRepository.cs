using eCommerce.Orders.Core.Objects.DbTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.Orders.Core.Contracts.Repositories
{
    public interface IOrderRepository
    {
        Task CreateOrderAsyn(OrderEntity orderEntity);
        IEnumerable<OrderEntity> GetOrdersByCustomer(string customerId, int orderStatus);
        Task<OrderEntity?> GetOrderByIdAsync(string orderId);
        Task UpdateOrderStatusAsync(OrderEntity orderEntity);
    }
}
