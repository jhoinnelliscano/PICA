
using eCommerce.Orders.Core.Objects.DbTypes;
using eCommerce.Orders.Infraestructure.Contexts.DbOrder;

namespace eCommerce.Orders.Infraestructure.DbHelper
{
    public interface IDbOrderHelper
    {
        OrderEntity ConvertToOrderEntity(Order order, bool addItems = false);
        Order ConvertToOrder(OrderEntity orderEntity);
    }
}
