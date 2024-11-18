using eCommerce.Commons.Objects.Messaging;
using eCommerce.Orders.Core.Config;
using eCommerce.Orders.Core.Contracts.Repositories;
using eCommerce.Orders.Core.Contracts.Services;
using eCommerce.Orders.Core.Helpers.Mappers;
using eCommerce.Orders.Core.Objects.DbTypes;
using eCommerce.Orders.Core.Objects.Dtos;
using eCommerce.Orders.Infraestructure.Models.UnitOfWorks;
using eCommerce.PublisherSubscriber.Contracts;
using eCommerce.PublisherSubscriber.Object;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace eCommerce.Orders.Infraestructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapperHelper _mapper;
        private readonly IPublisher _publisherMessage;
        private readonly IConfiguration _configuration;

        public OrderService(IOrderRepository orderRepository, IUnitOfWork unitOfWork,IMapperHelper mapperHelper,
            IPublisher publisherMessage, IConfiguration configuration)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapperHelper;
            _publisherMessage = publisherMessage;
            _configuration = configuration;
        }

        public async Task<string> Create(OrderDto order)
        {
            var orderId = Guid.NewGuid().ToString();

            if (order != null)
            {
                var orderAddressEntity = _mapper.MappToOrderShippingAddressEntity(order.ShippingAddress);
                var orderDetailEntity = order.OrderDetail.Select(x => _mapper.MappToOrderDetailEntity(x));
     
                var orderEntity = new OrderEntity(orderId, order.CustomerId, order.CustomerEmail, 1, order.Comment,
                    null, orderAddressEntity, orderDetailEntity.ToList());

                orderEntity.Total = orderDetailEntity.Sum(x => x.TotalByItem);

                order.Id = orderId;

                await  _orderRepository.CreateOrderAsyn(orderEntity);
                await _unitOfWork.ConfirmAsync();
            //    await SentOrderCreateMessage(order);
                await SentNotificationMessage(order);

                return orderId;
            }
            return string.Empty;
        }

        public async Task<IEnumerable<OrderDto>?> GetOrdersByCustomer(string customerId, int orderStatus)
        {
            var result = _orderRepository.GetOrdersByCustomer(customerId, orderStatus);

            if (result != null)
                return result.Select(x => new OrderDto(x.Id, x.CustomerId, x.CustomerEmail, x.OrderStatusId, x.OrderStatusDesc,
                    x.Comment, x.PaymentRef, x.Date, x.Total));
            
            return null;
        }

        public async Task<OrderDto?> GetOrderByIdAsync(string orderId)
        {
            var result = await _orderRepository.GetOrderByIdAsync(orderId);

            if (result != null)
            {
                var order = new OrderDto(result.Id, result.CustomerId, result.CustomerEmail, result.OrderStatusId, result.OrderStatusDesc,
                    result.Comment, result.PaymentRef, result.Date, result.Total);


                if(result.ShippingAddress != null)
                    order.ShippingAddress = new OrderShippingAddressDto(result.ShippingAddress.Id, result.ShippingAddress.OrderId, result.ShippingAddress.FirstName,
                        result.ShippingAddress.LastName, result.ShippingAddress.Email, result.ShippingAddress.Phone, result.ShippingAddress.Address,
                        result.ShippingAddress.AddressDesc, result.ShippingAddress.OtherInformation, result.ShippingAddress.City, result.ShippingAddress.Deparment, result.ShippingAddress.PostalCode);

                if (result.Detail != null)
                    order.OrderDetail = result.Detail.Select(x => new OrderDetailDto(x.Id, x.OrderId, x.ProductId, x.ShoppingCartId, x.ProductName, x.ProductDescription,
                    x.Quantity, x.Price));

                return order;
            }
            return null;
        }

        public async Task UpdateOrderStatusAsync(OrderDto order) 
        {
            var dbOrder = await GetOrderByIdAsync(order.Id);
            var orderEntity = new OrderEntity()
            {
                Id = order.Id,
                OrderStatusId = order.OrderStatusId,
                PaymentRef = order.PaymentRef,
            };

            if (order.PaymentData != null)
            {
                orderEntity.PaymentData = new OrderPaymentEntity()
                {
                    Amount = order.PaymentData.Amount,
                    CodRespuesta = order.PaymentData.CodRespuesta,
                    CustIdCliente = order.PaymentData.CustIdCliente,
                    CustomerIp = order.PaymentData.CustomerIp,
                    FechaTransaccion = order.PaymentData.FechaTransaccion,
                    Franchise = order.PaymentData.Franchise,
                    RefPayco = order.PaymentData.RefPayco,
                    ResponseReasonText = order.PaymentData.ResponseReasonText,
                    TransactionId = order.PaymentData.TransactionId,
                    TypePayment = order.PaymentData.TypePayment
                };
            }

            await _orderRepository.UpdateOrderStatusAsync(orderEntity);
            await _unitOfWork.ConfirmAsync();

            if(dbOrder!= null && order.OrderStatusId == Convert.ToInt32(_configuration.GetValue<string>("AppConfiguration:OrderStatusAccepted")))
                await SentOrderCreateMessage(dbOrder);
        }

        private Task SentOrderCreateMessage(OrderDto Order) 
        {
            var orderMsg = new OrderMsg()
            {
                CustomerId = Order.CustomerId,
                Products = Order.OrderDetail.Select(x => new OrderProductMsg(Convert.ToInt64(x.ProductId), x.Quantity))
            };
            var mqMessage = new Message<OrderMsg>("Purchase order created", orderMsg);
            var message = JsonConvert.SerializeObject(mqMessage);

            _publisherMessage.DistributeMessage(message, _configuration.GetValue<string>("AppConfiguration:MQ:PurchaseOrderQueue"));
            return Task.CompletedTask;
        }

        private Task SentNotificationMessage(OrderDto Order)
        {
            var messageBody = "Estimado {CUSTOMER} se ha generado una órden de compra con número {ORDERID}.";
            messageBody = messageBody.Replace("{CUSTOMER}", Order.CustomerName);
            messageBody = messageBody.Replace("{ORDERID}", Order.Id);

            var notificationMsg = new NotificationMsg()
            {
                Subject = "Su orden de compra ha sido creada",
                CustomerEmail = Order.CustomerEmail,
                CustomerName = Order.CustomerName,
                Body = messageBody
            };
            var mqMessage = new Message<NotificationMsg>("Purchase order created", notificationMsg);
            var message = JsonConvert.SerializeObject(mqMessage);

            _publisherMessage.PublishMessage(message, _configuration.GetValue<string>("AppConfiguration:MQ:NotificationQueue"));
            return Task.CompletedTask;
        }

    }
}
