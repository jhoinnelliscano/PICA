using eCommerce.Commons.Objects.Requests.Order;
using eCommerce.Commons.Objects.Responses.Order;
using eCommerce.Orders.Core.Objects.DbTypes;
using eCommerce.Orders.Core.Objects.Dtos;


namespace eCommerce.Orders.Core.Helpers.Mappers
{
    public interface IMapperHelper
    {
        OrderShippingAddressEntity MappToOrderShippingAddressEntity(OrderShippingAddressDto orderAddress);
        OrderDetailEntity MappToOrderDetailEntity(OrderDetailDto orderDetailDto);
        OrderResponse MappToOrderResponse(OrderDto orderDto);
        OrderDto MappToOrderOrderDto(CreateOrderRequest orderDto);
    }
}
