using eCommerce.Blazor.Demo.Pages.Shared;
using eCommerce.Commons.Objects.Requests.Products;
using eCommerce.Commons.Objects.Responses.Products;
using eCommerce.Services.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace eCommerce.Blazor.Demo.Pages.ProductManager
{
    public partial class ProductManager2 : ComponentBase
    {
        #region component parameters and Inject
        [CascadingParameter] protected MainLayout MainLayout { get; set; }
        [CascadingParameter] private Error Error { get; set; }
        [CascadingParameter] private Task<AuthenticationState> AuthenticationStateTask { get; set; }
        [Inject] ISnackbar Snackbar { get; set; }
        [Inject] private ProductManagerService ProductManagerService { get; set; }
        [Inject] private ProductService ProductService { get; set; }
        #endregion

        #region component properties
        private bool Procesing { get; set; }
        private bool ProductFound { get; set; }
        private bool ConfirmModal { get; set; } = false;
        private UpdateProductRequest UpdateProductRequest { get; set; }
        private ProductResponse Product { get; set; } = null;
        private string PorductCode { get; set; }
        private string PorductPrice { get; set; }
        private string PorductStock { get; set; }
        #endregion

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var role = await GetRole();
                if(!role.Equals("Administrator"))
                    NavManager.NavigateTo($"\\shop");

                MainLayout.Title = "Product Administrator";
                Procesing = false;
            }
            catch (Exception ex)
            {
                Error.ProcessError(ex);
            }
        }

        protected async Task GetProduct()
        {
            try
            {
                var request = new ProductRequest
                {
                    ProductCode = !string.IsNullOrEmpty(PorductCode) ? Convert.ToInt64(PorductCode) : 0
                };
                var result = await ProductService.GetProduct(request);

                if (result != null && result.Response != null)
                {
                    Product = result.Response;
                    PorductPrice = Product.Price.ToString();
                    PorductStock = Product.Stock.ToString();
                    ProductFound = true;
                }
                else
                {
                    ProductFound = false;
                    Snackbar.Add("Product not found", Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Error.ProcessError(ex);
            }
        }

        protected async Task UpdateProduct()
        {
            try
            {
                Procesing = true;
                UpdateProductRequest.ProductCode = Product.Code;
                UpdateProductRequest.Price = Convert.ToDecimal(PorductPrice);
                UpdateProductRequest.Stock = Convert.ToInt32(PorductStock);
                var result = await ProductManagerService.UpdateProduct(UpdateProductRequest);

                if (result.Response)
                {
                    Product.Price = UpdateProductRequest.Price;
                    Product.Stock = UpdateProductRequest.Stock;
                    Snackbar.Add("Product updated", Severity.Success);
                }
                else
                    Snackbar.Add("Product could not be updated", Severity.Error);
            }
            catch (Exception ex)
            {
                Error.ProcessError(ex);
            }
            finally
            {
                Procesing = false;
                ConfirmModal = false;
            }
        }

        protected Task ShowConfirmModal()
        {
            ConfirmModal = true;
            return Task.CompletedTask;
        }

        public async Task<string> GetRole()
        {
            var role = "Anonymous";
            try
            {
                var user = (await AuthenticationStateTask).User;
                if (user != null)
                {
                    role = user.Claims.FirstOrDefault(x => x.Type == "https://ecommerce.com/roles/role").Value;
                }
            }
            catch
            {
                role = "Anonymous";
            }
            return role;
        }
    }
}
