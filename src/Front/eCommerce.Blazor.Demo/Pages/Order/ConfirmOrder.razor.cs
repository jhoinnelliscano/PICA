using eCommerce.Blazor.Demo.Pages.Shared;
using eCommerce.Commons.Objects.Requests.Order;
using eCommerce.Commons.Objects.Responses.Order;
using eCommerce.Services.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace eCommerce.Blazor.Demo.Pages.Order
{
    public partial class ConfirmOrder : ComponentBase
    {
        [CascadingParameter] public MainLayout mainLayout { get; set; }
        [CascadingParameter] public Error Error { get; set; }
        [CascadingParameter] public EpayCo EpayCo { get; set; }
        [Parameter] public string OrderId { get; set; }
        [Parameter][SupplyParameterFromQuery(Name = "ref_payco")] public string? PaymenRef { get; set; }
        [Inject] private OrderService OrderService { get; set; }
        EpayCoResponse epayCoResponse { get; set; }

        OrderResponse order { get; set; }
        bool cargando = true;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                mainLayout.Title = "Respuesta de la Transacción";

                if (!string.IsNullOrEmpty(PaymenRef))
                {
                    await OnProcessPaymentRef();
                }
            }
            catch (Exception ex)
            {
                Error.ProcessError(ex);
            }
        }
        protected async Task OnProcessPaymentRef()
        {
            try
            {
                cargando = true;
                order = null;

                if (!PaymenRef.Equals("undefined"))
                {
                    epayCoResponse = await EpayCo.getTransaction(PaymenRef);
                    var updateOrder = await OrderService.UpdateOrderStatus(new Commons.Objects.Requests.Order.UpdateOrderStatusRequest()
                    {
                        OrderId = epayCoResponse.data.x_id_invoice,
                        OrderStatusId = epayCoResponse.data.x_cod_transaction_state,
                        PaymentRef = PaymenRef,
                        PaymentData = epayCoResponse.data == null ? new PaymentRefRequest() : new PaymentRefRequest
                        {
                            x_amount = epayCoResponse.data.x_amount.ToString(),
                            x_cod_respuesta = epayCoResponse.data.x_cod_respuesta,
                            x_customer_ip = epayCoResponse.data.x_customer_ip,
                            x_cust_id_cliente = epayCoResponse.data.x_cust_id_cliente.ToString(),
                            x_fecha_transaccion = epayCoResponse.data.x_fecha_transaccion,
                            x_franchise = epayCoResponse.data.x_franchise,
                            x_ref_payco = epayCoResponse.data.x_ref_payco.ToString(),
                            x_response_reason_text = epayCoResponse.data.x_response_reason_text,
                            x_transaction_id = epayCoResponse.data.x_transaction_id,
                            x_type_payment = epayCoResponse.data.x_type_payment
                        }
                    });
                    if (updateOrder.Response)
                    {
                        order = (await OrderService.GetOrder(new GetOrderRequest { OrderId = epayCoResponse.data.x_id_invoice })).Response;
                    }
                }
                else 
                {
                    var updateOrder = await OrderService.UpdateOrderStatus(new Commons.Objects.Requests.Order.UpdateOrderStatusRequest()
                    {
                        OrderId = OrderId,
                        OrderStatusId = 4,
                        PaymentData = new PaymentRefRequest()
                    });
                }
            }
            catch (Exception ex)
            {
                Error.ProcessError(ex);
            }
            finally
            {
                cargando = false;
            }
        }

    }
}
