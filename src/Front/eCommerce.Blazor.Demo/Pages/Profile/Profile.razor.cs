using eCommerce.Blazor.Demo.Pages.Shared;
using eCommerce.Commons.Objects.Requests.Customer;
using eCommerce.Commons.Objects.Responses.Customer;
using eCommerce.Commons.Utilities;
using eCommerce.Services.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace eCommerce.Blazor.Demo.Pages.Profile
{
    public partial class Profile : ComponentBase
    {
        #region component parameters and Inject
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }
        [CascadingParameter] private MainLayout MainLayout { get; set; }
        [CascadingParameter] private Error Error { get; set; }
        [CascadingParameter] private Task<AuthenticationState> AuthenticationStateTask { get; set; }
        [Inject] private CustomerService CustomerService { get; set; }
        [Inject] ISnackbar Snackbar { get; set; }
        #endregion

        #region component properties
        private bool Success { get; set; }
        private bool SuccessAddress { get; set; }
        private bool IsAgree { get; set; }
        private bool RegCompleted { get; set; }
        private bool SavingCustomer { get; set; } = false;
        private bool EditingMode { get; set; } = false;
        private bool SavingAddress { get; set; } = false;
        private bool DeletingAddress { get; set; } = false;
        private bool ShowModal { get; set; } = false;
        private bool ShowDeleteModal { get; set; } = false;
        private long IdShippingAddress { get; set; }

        private DialogOptions dialogOptions = new()
        {
            FullWidth = true,
            CloseOnEscapeKey = false,
            DisableBackdropClick = true,
            MaxWidth = MaxWidth.Medium
        };

        private DialogOptions deleteDialogOptions = new()
        {
            FullWidth = true,
            CloseOnEscapeKey = false,
            DisableBackdropClick = true,
            MaxWidth = MaxWidth.Small
        };

        private MudForm? form;

        private MudForm? formShippingAddress;

        string[] errors = { };
        private bool Searching { get; set; }
        private CreateCustomerRequest Customer { get; set; } = new();
        private CreateCustomerAddressRequest CreateCustomerAddress { get; set; } = new();
        private CustomerAddressRequest CustomerAddress { get; set; } = new();
        private IEnumerable<CustomerAddressResponse> CustomerAddressList { get; set; }
        private IEnumerable<CustomerIdentResponse> IdentificationsType { get; set; }
        #endregion

        protected override async Task OnInitializedAsync()
        {
            try
            {
                MainLayout.Title = "Profile";
                Searching = true;
                await GetIdentificationsType();
                await GetUserInformation();
                await GetShippingAddress();
                Searching = false;
            }
            catch (Exception ex)
            {
                Error.ProcessError(ex);
            }
        }

        private async Task GetIdentificationsType()
        {
            var servResponse = await CustomerService.GetIdentificationsType();
            IdentificationsType = servResponse.Response;
        }

        private async Task GetUserInformation()
        {
            try
            {
                var dbCutomer = await CustomerService.GetCustomer();

                if (dbCutomer.Response.RegCompleted)
                {
                    RegCompleted = true;
                    Customer.FirstName = dbCutomer.Response.FirstName;
                    Customer.LastName = dbCutomer.Response.LastName;
                    Customer.Email = dbCutomer.Response.Email;
                    Customer.AutenticationType = dbCutomer.Response.AutenticationType;
                    Customer.Identification = dbCutomer.Response.Identification;
                    Customer.IdentTypeId = dbCutomer.Response.IdentTypeId;
                }
                else
                {
                    RegCompleted = false;
                    var authState = await AuthenticationStateTask;
                    var user = authState.User;
                    var authType = UserUtilities.GetUserAuthenticationType(user.Claims.FirstOrDefault(c => c.Type == "sub").Value);
                    Customer.AutenticationType = authType;
                    Customer.Email = dbCutomer.Response.Email;
                    if (authType == "google-oauth2")
                    {
                        Customer.FirstName = user.Claims.FirstOrDefault(c => c.Type == "given_name").Value ?? "";
                        Customer.LastName = user.Claims.FirstOrDefault(c => c.Type == "family_name").Value ?? "";
                    }
                }
            }
            catch (Exception ex)
            {
                Error.ProcessError(ex);
            }
        }

        private async Task SaveUser()
        {
            try
            {
                SavingCustomer = true;
                await form.Validate();
                if (Success)
                {
                    if (!RegCompleted)
                    {
                        {
                            var authState = await AuthenticationStateTask;
                            var user = authState.User;
                            var userId = UserUtilities.GetUserId(user.Claims.FirstOrDefault(c => c.Type == "sub").Value);
                            Customer.Phone1 = "1111111111";
                            await CustomerService.CreateCustomer(Customer);
                            Snackbar.Add("User information updated", Severity.Success);
                        }

                        RegCompleted = true;
                    }
                    else
                    {
                        var request = new CustomerRequest
                        {
                            FirstName = Customer.FirstName,
                            LastName = Customer.LastName,
                            Identification = Customer.Identification,
                            IdentTypeId = Customer.IdentTypeId,
                            Email = Customer.Email,
                            AutenticationType = Customer.AutenticationType
                        };
                        await CustomerService.UpdateCustomer(request);
                        Snackbar.Add("User information updated", Severity.Success);
                    }
                }
            }
            catch (Exception ex)
            {
                Error.ProcessError(ex);
            }
            finally
            {
                SavingCustomer = false;
            }
        }

        private async Task CreateShippingAddress()
        {
            try
            {
                SavingAddress = true;
                await formShippingAddress.Validate();
                if (SuccessAddress)
                {
                    await CustomerService.CreateAddress(CreateCustomerAddress);
                    await GetShippingAddress();
                    ShowModal = false;
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

        private Task CloseShippingAddressModal()
        {
            ShowModal = false;
            return Task.CompletedTask;
        }

        private Task CloseDeleteShippingAddressModal()
        {
            ShowDeleteModal = false;
            return Task.CompletedTask;
        }

        private Task ShowCreateShippingAddressModal()
        {
            EditingMode = false;
            ShowModal = true;
            CreateCustomerAddress = new();
            return Task.CompletedTask;
        }

        private async Task GetShippingAddress()
        {
            var result = await CustomerService.GetAddress();
            CustomerAddressList = result.Response;
        }

        private Task ShowDeleteShippingAddressModel(long idShippingAddress)
        {
            ShowDeleteModal = true;
            IdShippingAddress = idShippingAddress;
            return Task.CompletedTask;
        }

        private async Task DeleteShippingAddress()
        {
            CustomerAddress = new();
            CustomerAddress.Id = IdShippingAddress;
            CustomerAddress.Inactive = true;
            await CustomerService.UpdateAddress(CustomerAddress);
            await GetShippingAddress();
            ShowDeleteModal = false;
            Snackbar.Add("Shipping address deleted", Severity.Success);
        }

        private async Task ShowEditShippingAddressModal(long idShippingAddress)
        {
            EditingMode = true;
            ShowModal = true;
            var result = CustomerAddressList.FirstOrDefault(x => x.Id == idShippingAddress);

            CreateCustomerAddress = new();
            CreateCustomerAddress.Id = idShippingAddress;
            CreateCustomerAddress.FirstName = result.FirstName;
            CreateCustomerAddress.LastName = result.LastName;
            CreateCustomerAddress.Email = result.Email;
            CreateCustomerAddress.Phone = result.Phone;
            CreateCustomerAddress.Address = result.Address;
            CreateCustomerAddress.AddressDesc = result.AddressDesc;
            CreateCustomerAddress.City = result.City;
            CreateCustomerAddress.FirstName = result.FirstName;
            CreateCustomerAddress.Deparment = result.Deparment;
            CreateCustomerAddress.Default = result.Default;
            CreateCustomerAddress.OtherInformation = result.OtherInformation;
        }

        private async Task UpdateShippingAddress()
        {

            try
            {
                SavingAddress = true;
                CustomerAddress = new();
                CustomerAddress.Id = CreateCustomerAddress.Id;
                CustomerAddress.FirstName = CreateCustomerAddress.FirstName;
                CustomerAddress.LastName = CreateCustomerAddress.LastName;
                CustomerAddress.Email = CreateCustomerAddress.Email;
                CustomerAddress.Phone = CreateCustomerAddress.Phone;
                CustomerAddress.Address = CreateCustomerAddress.Address;
                CustomerAddress.AddressDesc = CreateCustomerAddress.AddressDesc;
                CustomerAddress.City = CreateCustomerAddress.City;
                CustomerAddress.FirstName = CreateCustomerAddress.FirstName;
                CustomerAddress.Deparment = CreateCustomerAddress.Deparment;
                CustomerAddress.Default = CreateCustomerAddress.Default;

                await CustomerService.UpdateAddress(CustomerAddress);
                await GetShippingAddress();
                ShowModal = false;
                Snackbar.Add("Shipping address updated", Severity.Success);
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
    }
}
