

namespace eCommerce.Commons.Objects.Requests.Order
{
    public class PaymentRefRequest
    {
        public string x_ref_payco { get; set; } = "";
        public string x_cust_id_cliente { get; set; } = "";
        public string x_transaction_id { get; set; } = "";
        public string x_fecha_transaccion { get; set; } = "";
        public int x_cod_respuesta { get; set; }
        public string x_response_reason_text { get; set; } = "";
        public string x_franchise { get; set; } = "";
        public string x_customer_ip { get; set; } = "";
        public string x_type_payment { get; set; } = "";
        public string x_amount { get; set; } = "";
    }
}
