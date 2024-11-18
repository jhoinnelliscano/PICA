using eCommerce.Commons.Objects.Requests.Order;
using eCommerce.Commons.Objects.Responses.Order;
using eCommerce.Orders.Core.Objects.DbTypes;
using eCommerce.Orders.Core.Objects.Dtos;
namespace eCommerce.Orders.Core.Helpers.Mappers
{
    public class MapperHelper : IMapperHelper
    {
       
        public OrderShippingAddressEntity MappToOrderShippingAddressEntity(OrderShippingAddressDto orderAddress)
        {
            return new OrderShippingAddressEntity(0, orderAddress.OrderId, orderAddress.FirstName, orderAddress.LastName
                , orderAddress.Email, orderAddress.Phone, orderAddress.Address, orderAddress.AddressDesc, orderAddress.OtherInformation,
                orderAddress.City, orderAddress.Deparment, orderAddress.PostalCode);
        }

        public OrderDetailEntity MappToOrderDetailEntity(OrderDetailDto orderDetailDto)
        {
            return new OrderDetailEntity(orderDetailDto.Id, orderDetailDto.OrderId, orderDetailDto.ProductId, orderDetailDto.ShoppingCartId,
                orderDetailDto.ProductName, orderDetailDto.ProductDescription, orderDetailDto.Quantity, orderDetailDto.Price);
        }

        public OrderResponse MappToOrderResponse(OrderDto orderDto) 
        {
            var order = new OrderResponse()
            {
                Id = orderDto.Id,
                Comment = orderDto.Comment,
                CustomerEmail = orderDto.CustomerEmail,
                CustomerId = orderDto.CustomerId,
                Date = orderDto.Date,
                OrderStatusId = orderDto.OrderStatusId,
                OrderStatusDesc = orderDto.OrderStatusDesc,
                Total = orderDto.Total,
                PaymentRef = orderDto.PaymentRef
            };

            if (orderDto.ShippingAddress != null)
                order.ShippingAddress = new OrderAddressResponse()
                {
                    Id = orderDto.ShippingAddress.Id,
                    Address = orderDto.ShippingAddress.Address,
                    AddressDesc = orderDto.ShippingAddress.AddressDesc,
                    City = orderDto.ShippingAddress.City,
                    Deparment = orderDto.ShippingAddress.Deparment,
                    Email = orderDto.ShippingAddress.Email,
                    FirstName = orderDto.ShippingAddress.FirstName,
                    LastName = orderDto.ShippingAddress.LastName,
                    OrderId = orderDto.ShippingAddress.OrderId,
                    OtherInformation = orderDto.ShippingAddress.OtherInformation,
                    Phone = orderDto.ShippingAddress.Phone,
                    PostalCode = orderDto.ShippingAddress.PostalCode,
                };

            if (orderDto.OrderDetail != null)
                order.OrderDetail = orderDto.OrderDetail.Select(x => ConvertToOrderDetailResponse(x)).ToList();

            return order;
        }

        public OrderDto MappToOrderOrderDto(CreateOrderRequest orderReq)
        {
            var order = new OrderDto()
            {
                Comment = orderReq.Comment,
                CustomerEmail = orderReq.CustomerEmail,
                CustomerId = orderReq.CustomerId,
                CustomerName = !string.IsNullOrEmpty(orderReq.CustomerName) ? orderReq.CustomerName : "Cliente",
                ShippingAddress = 
                    new OrderShippingAddressDto()
                    {
                        Address = orderReq.ShippingAddress.Address,
                        AddressDesc = orderReq.ShippingAddress.AddressDesc,
                        City = orderReq.ShippingAddress.City,
                        Deparment = orderReq.ShippingAddress.Deparment,
                        Email = orderReq.ShippingAddress.Email,
                        FirstName = orderReq.ShippingAddress.FirstName,
                        LastName = orderReq.ShippingAddress.LastName,
                        OrderId = orderReq.ShippingAddress.OrderId,
                        OtherInformation = orderReq.ShippingAddress.OtherInformation,
                        Phone = orderReq.ShippingAddress.Phone,
                        PostalCode = orderReq.ShippingAddress.PostalCode,
                    },
                OrderDetail = orderReq.OrderDetail.Select(x => ConvertToOrderDetailDto(x)).ToList()

            };

            return order;
        }

        private OrderDetailDto ConvertToOrderDetailDto(OrderDetailRequest detail)
        {
            return new OrderDetailDto()
            {
                Price = detail.Price,
                ShoppingCartId = detail.ShoppingCartId,
                ProductDescription = detail.ProductName,
                ProductId = detail.ProductId,
                ProductName = detail.ProductName,
                Quantity = detail.Quantity,
            };
        }

        private OrderDetailResponse ConvertToOrderDetailResponse(OrderDetailDto detail)
        {
            return new OrderDetailResponse()
            {
                Id = detail.Id,
                Price = detail.Price,
                ShoppingCartId = detail.ShoppingCartId,
                ProductDescription = detail.ProductName,
                ProductId = detail.ProductId,
                ProductName = detail.ProductName,
                Quantity = detail.Quantity,
            };
        }

    }
}
