
namespace eCommerce.Orders.Core.Objects.DbTypes
{
    public class OrderPaymentEntity
    {
        public string RefPayco { get; set; }
        public string CustIdCliente { get; set; }
        public string TransactionId { get; set; }
        public string FechaTransaccion { get; set; }
        public int CodRespuesta { get; set; }
        public string ResponseReasonText { get; set; }
        public string Franchise { get; set; }
        public string CustomerIp { get; set; }
        public string TypePayment { get; set; }
        public string Amount { get; set; }
    }
}
