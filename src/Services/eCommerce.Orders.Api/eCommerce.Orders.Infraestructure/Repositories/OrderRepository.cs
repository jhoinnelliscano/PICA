using eCommerce.Orders.Core.Contracts.Repositories;
using eCommerce.Orders.Core.Objects.DbTypes;
using eCommerce.Orders.Infraestructure.Contexts.DbOrder;
using eCommerce.Orders.Infraestructure.DbHelper;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.Orders.Infraestructure.Repositories
{
    public class OrderRepository: IOrderRepository
    {
        private readonly DbOrderContext _dbcontext;
        private readonly IDbOrderHelper _dbOrderHelper;

        public OrderRepository(DbOrderContext dbcontext, IDbOrderHelper dbOrderHelper)
        {
            _dbcontext = dbcontext;
            _dbOrderHelper = dbOrderHelper;
        }

        public async Task CreateOrderAsyn(OrderEntity orderEntity)
        {
           var order = _dbOrderHelper.ConvertToOrder(orderEntity);
           order.OrderStatusId = 3;
           order.Date = DateTime.Now;
           await _dbcontext.Orders.AddAsync(order); 
        }
   
        public IEnumerable<OrderEntity> GetOrdersByCustomer(string customerId, int orderStatus)
        {
            if (orderStatus > 0)
            {
                var resultWithStatus = _dbcontext.Orders.Include(x => x.OrderStatus)
                                .Where(x => x.CustomerId.Equals(customerId) && x.OrderStatusId == orderStatus)
                                .OrderByDescending(x => x.Date)
                                .ToList();

                if (resultWithStatus != null)
                    return resultWithStatus.Select(x => _dbOrderHelper.ConvertToOrderEntity(x));               
            }

            var result = _dbcontext.Orders.Include(x => x.OrderStatus)
                                    .Where(x => x.CustomerId.Equals(customerId))
                                    .OrderByDescending(x => x.Date)
                                    .ToList();

            if (result != null)
                return result.Select(x => _dbOrderHelper.ConvertToOrderEntity(x));

            return null;
        }

        public async Task<OrderEntity?> GetOrderByIdAsync(string orderId)
        {
            var result = await _dbcontext.Orders
                                   .Include(x => x.OrderDetails)
                                   .Include(x => x.OrderShippingAddresses)
                                   .Include(x => x.OrderStatus)
                                   .FirstOrDefaultAsync(x=>x.Id.Equals(orderId));
            if (result != null)
                return _dbOrderHelper.ConvertToOrderEntity(result, addItems: true);

            return null;
        }

        public async Task UpdateOrderStatusAsync(OrderEntity orderEntity)
        {
            var order = await _dbcontext.Orders.FindAsync(orderEntity.Id);

            if (order != null)
            {
                order.OrderStatusId = orderEntity.OrderStatusId;

                if (!string.IsNullOrEmpty(orderEntity.PaymentRef))
                    order.PaymentRef = orderEntity.PaymentRef;

                if (orderEntity.PaymentRef != null)
                {
                    var paymentRef = new OrderPayment()
                    {
                        Amount = orderEntity.PaymentData.Amount,
                        ResponseCode = orderEntity.PaymentData.CodRespuesta.ToString(),
                        CustIdCliente = orderEntity.PaymentData.CustIdCliente,
                        CustomerIp = orderEntity.PaymentData.CustomerIp,
                        TransactionDate = orderEntity.PaymentData.FechaTransaccion,
                        PaymentFranchiseId = orderEntity.PaymentData.Franchise,
                        RefPayco = orderEntity.PaymentData.RefPayco,
                        ResponseReason = orderEntity.PaymentData.ResponseReasonText,
                        TransactionId = orderEntity.PaymentData.TransactionId,
                        PaymentTypeId = orderEntity.PaymentData.TypePayment
                    };
                    order.OrderPayments.Add(paymentRef);
                }
            }
        }
    }
}
