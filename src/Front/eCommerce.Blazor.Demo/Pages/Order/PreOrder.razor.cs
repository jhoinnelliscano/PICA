using eCommerce.Blazor.Demo.Pages.Shared;
using eCommerce.Blazor.Demo.SessionStorage;
using eCommerce.Commons.Objects.Requests.Customer;
using eCommerce.Commons.Objects.Requests.Order;
using eCommerce.Commons.Objects.Requests.Payments;
using eCommerce.Commons.Objects.Responses.Customer;
using eCommerce.Commons.Objects.Responses.Order;
using eCommerce.Commons.Objects.Responses.ShoppingCart;
using eCommerce.Commons.Utilities;
using eCommerce.Services.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;

namespace eCommerce.Blazor.Demo.Pages.Order
{
    public partial class PreOrder : ComponentBase
    {
        #region component parameters and Inject
        [CascadingParameter] public MainLayout mainLayout { get; set; }
        [CascadingParameter] public Error Error { get; set; }
        [CascadingParameter] public EpayCo EpayCo { get; set; }
        [Parameter] public string Code { get; set; }
        [Inject] public ShoppingSessionStorage _ShoppingSessionStorage { get; set; }
        [Inject] public IConfiguration _Configuration { get; set; }
        [Inject] public IJSRuntime _js { get; set; }
        [Inject] private CustomerService CustomerService { get; set; }
        [Inject] private OrderService OrderService { get; set; }
        [Inject] ISnackbar Snackbar { get; set; }
        [Inject] PaymentEpayCo Payment { get; set; }
        #endregion

        #region component properties
        private CustomerAddressResponse CurrentAddress { get; set; }
        private CustomerAddressRequest OperatorAddress { get; set; }
        private IEnumerable<CustomerAddressResponse> CustomerAddressList { get; set; }
        private CustomerResponse customer { get; set; }
        private List<ShoppingCartResponse>? items { get; set; }
        public decimal TotalPriceIsCheck { get => items.Sum(x => x.TotalPrice); }
        public string textPrueba;
        public bool boolPrueba;
        protected bool isOpenAddressForm = false;
        protected DialogOptions dialogAddressOptions = new()
        {
            FullWidth = true,
            CloseOnEscapeKey = false,
            DisableBackdropClick = true,
            MaxWidth = MaxWidth.Large
        };
        protected bool isOpenAddressDeleteForm = false;
        protected DialogOptions dialogAddressDeleteOptions = new()
        {
            FullWidth = true,
            CloseOnEscapeKey = false,
            DisableBackdropClick = true,
            MaxWidth = MaxWidth.Large,
            NoHeader = true
        };

        private CartUtilities.OperationMode operation { get; set; } = CartUtilities.OperationMode.List;
        private MudForm? formShippingAddress;
        private bool SuccessAddress { get; set; }
        private bool EditingMode { get; set; } = false;
        private bool SavingAddress { get; set; } = false;
        private bool DeletingAddress { get; set; } = false;

        #endregion

        #region component events
        protected override async Task OnInitializedAsync()
        {
            try
            {
                mainLayout.Title = "Order";
                items = (await _ShoppingSessionStorage.GetShoppingCart()).ToList();
                await GetShippingAddress();

            }
            catch (Exception ex)
            {
                Error.ProcessError(ex);
            }
        }

        protected async Task OnToggleAddressForm()
        {
            isOpenAddressForm = !isOpenAddressForm;
        }


        protected async Task onShopChanged()
        {
            try
            {
                _ShoppingSessionStorage.SaveShoppingCart(items);
            }
            catch (Exception ex)
            {
                Error.ProcessError(ex);
            }
        }

        protected async Task checkOut(MouseEventArgs e)
        {
            try
            {
                var orden = await CreateOrder();
                Payment.name = (orden.OrderDetail.Count() > 1) ? "varios productos" : orden.Comment;//Varios productos
                Payment.description = orden.Comment;
                Payment.invoice = orden.Id;
                Payment.amount = orden.Total.ToString("G29");
                Payment.email_billing = orden.ShippingAddress.Email;
                Payment.name_billing = string.Concat(orden.ShippingAddress.FirstName, " ", orden.ShippingAddress.LastName);
                Payment.address_billing = string.Concat(orden.ShippingAddress.Address, ", ", orden.ShippingAddress.Address);
                Payment.mobilephone_billing = orden.ShippingAddress.Phone;
                var a = _Configuration.GetSection("EpayCo:methodsDisable").AsEnumerable();
                await EpayCo.checkout(Payment);
            }
            catch (Exception ex)
            {
                Error.ProcessError(ex);
            }
        }
        #endregion


        private async Task<OrderResponse> CreateOrder()
        {
            var request = new CreateOrderRequest()
            {
                CustomerId = customer.Id,
                CustomerEmail = customer.Email,
                CustomerName = $"{customer.FirstName} {customer.LastName}",
                Comment = string.Join(",", items.Select(x => x.Product.Name).ToArray()),
                ShippingAddress = new OrderShippingAddressRequest
                {
                    Id = CurrentAddress.Id,
                    FirstName = CurrentAddress.FirstName,
                    LastName = CurrentAddress.LastName,
                    Email = CurrentAddress.Email,
                    Phone = CurrentAddress.Phone,
                    Address = CurrentAddress.Address,
                    AddressDesc = CurrentAddress.AddressDesc,
                    City = CurrentAddress.City,
                    Deparment = CurrentAddress.Deparment,
                    OtherInformation = CurrentAddress.OtherInformation,
                    OrderId = "0"
                },
                OrderDetail = items.Select(x => new OrderDetailRequest
                {
                    ShoppingCartId = x.Id,
                    ProductId = x.Product.Code,
                    ProductName = x.Product.Name,
                    Price = x.Price,
                    Quantity = x.Quantity
                })
            };

            var createOrder = await OrderService.CreateOrder(request);

            OrderResponse order = null;

            if (createOrder.Response != null)
            {
                order = (await OrderService.GetOrder(new GetOrderRequest { OrderId = createOrder.Response.OrderId })).Response;
                _ShoppingSessionStorage.RemoveShoppingCart();
            }

            return order;
        }
        private async Task GetShippingAddress()
        {
            var result = await CustomerService.GetCustomer();
            customer = result.Response;
            var resulta = await CustomerService.GetAddress();
            CustomerAddressList = resulta.Response;
            CurrentAddress = CustomerAddressList.FirstOrDefault(x => x.Default);
        }

        private async Task ShowListShippingAddressModal()
        {
            operation = CartUtilities.OperationMode.List;
            isOpenAddressForm = true;
            EditingMode = false;
            DeletingAddress = false;
        }

        private async Task ShowCreateShippingAddressModal()
        {
            operation = CartUtilities.OperationMode.New;
            EditingMode = false;
            DeletingAddress = false;
            OperatorAddress = new();
        }

        private async Task ShowEditShippingAddressModal(long idShippingAddress)
        {
            operation = CartUtilities.OperationMode.Edit;
            EditingMode = true;
            DeletingAddress = false;
            OperatorAddress = CustomerAddressList.Where(x => x.Id == idShippingAddress).Select(r => new CustomerAddressRequest()
            {
                Id = r.Id,
                FirstName = r.FirstName,
                LastName = r.LastName,
                Email = r.Email,
                Phone = r.Phone,
                Address = r.Address,
                AddressDesc = r.AddressDesc,
                City = r.City,
                Deparment = r.Deparment,
                Default = r.Default,
                OtherInformation = r.OtherInformation
            }).First();
        }

        private async Task ShowDeleteShippingAddressModal(long idShippingAddress)
        {
            operation = CartUtilities.OperationMode.Delete;
            EditingMode = false;
            OperatorAddress = CustomerAddressList.Where(x => x.Id == idShippingAddress).Select(r => new CustomerAddressRequest()
            {
                Id = r.Id,
                FirstName = r.FirstName,
                LastName = r.LastName,
                Email = r.Email,
                Phone = r.Phone,
                Address = r.Address,
                AddressDesc = r.AddressDesc,
                City = r.City,
                Deparment = r.Deparment,
                Default = r.Default,
                OtherInformation = r.OtherInformation
            }).First();
        }

        private async Task ShowSelectShippingAddressModal(long idShippingAddress)
        {
            OperatorAddress = CustomerAddressList.Where(x => x.Id == idShippingAddress).Select(r => new CustomerAddressRequest()
            {
                Id = r.Id,
                FirstName = r.FirstName,
                LastName = r.LastName,
                Email = r.Email,
                Phone = r.Phone,
                Address = r.Address,
                AddressDesc = r.AddressDesc,
                City = r.City,
                Deparment = r.Deparment,
                Default = r.Default,
                OtherInformation = r.OtherInformation
            }).First();
        }

        private async Task CancelOperationShippingAddressModal()
        {
            if (operation == CartUtilities.OperationMode.Edit)
                OperatorAddress = CustomerAddressList.Where(x => x.Id == OperatorAddress.Id).Select(r => new CustomerAddressRequest()
                {
                    Id = r.Id,
                    FirstName = r.FirstName,
                    LastName = r.LastName,
                    Email = r.Email,
                    Phone = r.Phone,
                    Address = r.Address,
                    AddressDesc = r.AddressDesc,
                    City = r.City,
                    Deparment = r.Deparment,
                    Default = r.Default,
                    OtherInformation = r.OtherInformation
                }).First();
            operation = CartUtilities.OperationMode.List;
            EditingMode = false;
            DeletingAddress = false;
        }

        private async Task CloseModalShippingAddressModal()
        {
            operation = CartUtilities.OperationMode.List;
            OperatorAddress = null;
            EditingMode = false;
            DeletingAddress = false;
            isOpenAddressForm = false;
        }

        private async Task OkModalShippingAddressModal()
        {
            operation = CartUtilities.OperationMode.List;
            if (OperatorAddress != null)
                CurrentAddress = CustomerAddressList.FirstOrDefault(x => x.Id == OperatorAddress.Id);
            EditingMode = false;
            DeletingAddress = false;
            isOpenAddressForm = false;
        }

        private async Task CreateShippingAddress()
        {
            try
            {
                SavingAddress = true;
                await formShippingAddress.Validate();
                if (SuccessAddress)
                {
                    await CustomerService.CreateAddress(OperatorAddress);//
                    await GetShippingAddress();
                    operation = CartUtilities.OperationMode.List;
                    Snackbar.Add("Shipping address created", Severity.Success);
                }
            }
            catch (Exception ex)
            {
                Error.ProcessError(ex);
            }
            finally
            {
                SavingAddress = false;
            }
        }

        private async Task UpdateShippingAddress()
        {
            try
            {
                SavingAddress = true;
                await formShippingAddress.Validate();
                if (SuccessAddress)
                {
                    await CustomerService.UpdateAddress(OperatorAddress);
                    await GetShippingAddress();
                    operation = CartUtilities.OperationMode.List;
                    Snackbar.Add("Shipping address updated", Severity.Success);
                }
            }
            catch (Exception ex)
            {
                Error.ProcessError(ex);
            }
            finally
            {
                SavingAddress = false;
            }
        }

        private async Task DeleteShippingAddress()
        {
            OperatorAddress.Inactive = true;
            await CustomerService.UpdateAddress(OperatorAddress);
            await GetShippingAddress();
            operation = CartUtilities.OperationMode.List;
            Snackbar.Add("Shipping address deleted", Severity.Success);
        }


    }
}
