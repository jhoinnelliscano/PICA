using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using eCommerce.Orders.Core.Contracts;
using eCommerce.Orders.Core.Contracts.Services;
using Microsoft.AspNetCore.Authorization;
using eCommerce.Orders.Core.Objects.Dtos;
using eCommerce.Orders.Core.Helpers.Mappers;
using eCommerce.Commons.Objects.Responses;
using eCommerce.Commons.Objects.Requests.Order;
using eCommerce.Commons.Objects.Responses.Order;

namespace eCommerce.Orders.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IOrderService _orderService;
        private IMapperHelper _mapperHelper;

        public OrderController(IOrderService orderService, ILogger<OrderController> logger,IMapperHelper mapperHelper)
        {
            _orderService = orderService;
            _logger = logger;
            _mapperHelper = mapperHelper;
        }
        
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ServiceResponse<CreateOrderResponse>>> Create([FromBody] CreateOrderRequest request)
        {
            var order = _mapperHelper.MappToOrderOrderDto(request);
            var result = await _orderService.Create(order);

            var response = string.IsNullOrEmpty(result) ? new CreateOrderResponse() : new CreateOrderResponse()
            {
                OrderId = result,
                Result = true
            };

            var message = string.IsNullOrEmpty(result) ? "Create.Order.Failed" : "Create.Order.Ok";
            var msgBody = new { orderId = result, customerId = request.CustomerId };
            _logger.LogInformation(message + " {msgBody} ", msgBody);

            foreach (var product in request.OrderDetail)
            {
                var msgBody2 = new { city = request.ShippingAddress.City, productId = product.ProductId };
                _logger.LogInformation("Order.City.Product {msgBody2}", msgBody2);
            }

            return Ok(new ServiceResponse<CreateOrderResponse>("Successful", response));
        }

        /// <summary>
        /// Consulta de ordenes por Cliente y/o estados de la orden
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ServiceResponse<IEnumerable<OrderResponse>>>> GetOrders([FromQuery] GetOrderByUserRequest request)
        {
            var result = await _orderService.GetOrdersByCustomer(request.CustomerId, request.OrderStatusId);

            if (result != null) 
            {
                var response = result.Select(x => _mapperHelper.MappToOrderResponse(x));
                return Ok(new ServiceResponse<IEnumerable<OrderResponse>>("Successful", response));
            }
            return Ok(new ServiceResponse<OrderResponse?>("Successful", null));
        }

        /// <summary>
        /// Consulta deldetalle de una orden Id orden
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Detail")]
        [Authorize]
        public async Task<ActionResult<ServiceResponse<OrderResponse>>> GetOrder([FromQuery] GetOrderRequest request)
        {
            var result = await _orderService.GetOrderByIdAsync(request.OrderId);
            if (result != null)
            {
                var response = _mapperHelper.MappToOrderResponse(result);
                return Ok(new ServiceResponse<OrderResponse>("Successful", response));
            }
            return Ok(new ServiceResponse<OrderResponse?>("Successful", null));
        }

        /// <summary>
        /// Actualización de estado de una orden
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize]
        public async Task<ActionResult<ServiceResponse<bool>>> UpdateOrderStatus([FromBody] UpdateOrderStatusRequest request)
        {
            var order = new OrderDto()
            {
                Id = request.OrderId,
                OrderStatusId = request.OrderStatusId,
                PaymentRef = request.PaymentRef,
            };

            if (request.PaymentData != null && !string.IsNullOrEmpty(request.PaymentData.x_transaction_id))
            {
                order.PaymentData = new OrderPaymentDto()
                {
                    Amount = request.PaymentData.x_amount,
                    CodRespuesta = request.PaymentData.x_cod_respuesta,
                    CustIdCliente = request.PaymentData.x_cust_id_cliente,
                    CustomerIp = request.PaymentData.x_customer_ip,
                    FechaTransaccion = request.PaymentData.x_fecha_transaccion,
                    Franchise = request.PaymentData.x_franchise,
                    RefPayco = request.PaymentData.x_franchise,
                    ResponseReasonText = request.PaymentData.x_response_reason_text,
                    TransactionId = request.PaymentData.x_transaction_id,
                    TypePayment = request.PaymentData.x_type_payment
                };
            }

            await _orderService.UpdateOrderStatusAsync(order);

            var msg = new { orderId = request.OrderId, statusId = request.OrderStatusId, paymentRef = request.PaymentRef };
            _logger.LogInformation("Update.OrderStatus.Ok {msg}", msg);
            _logger.LogInformation($"Payment.Ok.Status_" + request.OrderStatusId.ToString());

            if (request.PaymentData != null && !string.IsNullOrEmpty(request.PaymentData.x_transaction_id))
            {
                _logger.LogInformation($"Payment.Ok.Type_" + request.PaymentData.x_type_payment);
                _logger.LogInformation($"Payment.Ok.Type_" + request.PaymentData.x_franchise);
            }

            return Ok(new ServiceResponse<bool>("Successful", true));
        }

    }
}
