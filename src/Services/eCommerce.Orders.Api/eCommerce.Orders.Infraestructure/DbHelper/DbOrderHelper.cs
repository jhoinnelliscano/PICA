using eCommerce.Orders.Core.Objects.DbTypes;
using eCommerce.Orders.Infraestructure.Contexts.DbOrder;

namespace eCommerce.Orders.Infraestructure.DbHelper
{
    public class DbOrderHelper : IDbOrderHelper
    {
        public OrderEntity ConvertToOrderEntity(Order order, bool addItems = false) 
        {
            var orderEntity =new OrderEntity()
            {
                 Id = order.Id,
                 Comment = order.Comment,
                 CustomerEmail = order.CustomerEmail,
                 CustomerId = order.CustomerId,
                 Date = order.Date,
                 OrderStatusId = order.OrderStatusId,
                 OrderStatusDesc = order.OrderStatus == null ? "" : order.OrderStatus.Descripction,
                 Total = order.Total,
                 PaymentRef = order.PaymentRef,
            };

            if (addItems) 
            {
                orderEntity.Detail = order.OrderDetails.Select(x => ConvertToOrderDetailEntity(x));

                var address = order.OrderShippingAddresses.FirstOrDefault();
                orderEntity.ShippingAddress = address != null ? ConvertToOrderShippingAddressEntity(address) : new OrderShippingAddressEntity();
            }

            return orderEntity;
        }

        private OrderDetailEntity ConvertToOrderDetailEntity(OrderDetail detail) 
        {
            return new OrderDetailEntity()
            {
                 Id=detail.Id,
                 OrderId  = detail.OrderId,
                 Price = detail.Price,
                 ProductDescription = detail.ProductDescription,
                 ShoppingCartId = detail.IdShoppingCart != null ? Convert.ToInt64(detail.IdShoppingCart) : 0,
                 ProductId = detail.ProductId,
                 ProductName = detail.ProductName,
                 Quantity = detail.Quantity,
            };
        }

        private OrderShippingAddressEntity ConvertToOrderShippingAddressEntity(OrderShippingAddress address)
        {
            return new OrderShippingAddressEntity()
            {
                 Address = address.Address,
                 AddressDesc = address.AddressDesc,
                 City = address.City,
                 Deparment = address.Deparment,
                 Email = address.Email,
                 FirstName = address.FirstName,
                 LastName = address.LastName,  
                 Id = address.Id,
                 OrderId = address.OrderId,
                 OtherInformation = address.OtherInformation,
                 Phone = address.Phone,
                 PostalCode = address.PostalCode,
            };
        }

        public Order ConvertToOrder(OrderEntity orderEntity)
        {
            var order = new Order()
            {
                Id = orderEntity.Id,
                Comment = orderEntity.Comment,
                CustomerEmail = orderEntity.CustomerEmail,
                CustomerId = orderEntity.CustomerId,
                Date = orderEntity.Date,            
                OrderStatusId = orderEntity.OrderStatusId,               
                Total = orderEntity.Total,
                PaymentRef = orderEntity.PaymentRef,
                OrderShippingAddresses = new List<OrderShippingAddress>
                 {
                    new OrderShippingAddress() 
                    { 
                        Address = orderEntity.ShippingAddress.Address,
                        AddressDesc = orderEntity.ShippingAddress.AddressDesc,
                        City = orderEntity.ShippingAddress.City,
                        Deparment = orderEntity.ShippingAddress.Deparment,
                        Email = orderEntity.ShippingAddress.Email,
                        FirstName = orderEntity.ShippingAddress.FirstName,
                        LastName = orderEntity.ShippingAddress.LastName,  
                        OrderId = orderEntity.ShippingAddress.OrderId,
                        OtherInformation = orderEntity.ShippingAddress.OtherInformation,
                        Phone = orderEntity.ShippingAddress.Phone,
                        PostalCode = orderEntity.ShippingAddress.PostalCode,
                    }
                },
                OrderDetails = orderEntity.Detail.Select(x=> ConvertToOrderDetail(x)).ToList()

            };

            return order;
        }

        private OrderDetail ConvertToOrderDetail(OrderDetailEntity detail)
        {
            return new OrderDetail()
            {
                OrderId = detail.OrderId,
                Price = detail.Price,
                ProductDescription = detail.ProductDescription,               
                ProductId = detail.ProductId,
                IdShoppingCart = detail.ShoppingCartId,
                ProductName = detail.ProductName,
                Quantity = detail.Quantity,
            };
        }     
    }
}
