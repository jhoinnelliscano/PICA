using eCommerce.Blazor.Demo;
using eCommerce.Blazor.Demo.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Microsoft.Extensions.Http;
using eCommerce.Services.Services;
using Blazored.Toast;
using Blazored.LocalStorage;
using eCommerce.Blazor.Demo.Auth;
using eCommerce.Blazor.Demo.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using eCommerce.Blazor.Demo.SessionStorage;
using eCommerce.Services.Contracts;
using eCommerce.Commons.Objects.Requests.Payments;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

#region HttpServices
//builder.Services.AddScoped<AuthorizerUserService>();
//builder.Services.AddScoped<ProductService>();
//builder.Services.AddScoped<ShoppingService>();
builder.Services.AddScoped<AuthHttpMessageHandler>();
//builder.Services.AddScoped(sp => new HttpClient
//{
//    BaseAddress = new Uri(builder.Configuration["Service:BaseUrl"].ToString())
//});

builder.Services.AddHttpClient();
//builder.Services.AddHttpClient<AuthorizerUserService>(config =>
//        {
//            config.BaseAddress = new Uri(builder.Configuration["Auth:BaseUrl"].ToString());
//            config.Timeout = TimeSpan.FromSeconds(10000);
//            //config.DefaultRequestHeaders.Clear();
//            config.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
//        }
//    );

//builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddHttpClient<ProductService>(config =>
{
    config.BaseAddress = new Uri(builder.Configuration["Services:Products:BaseUrl"].ToString());
    config.Timeout = TimeSpan.FromSeconds(10000);
    //config.DefaultRequestHeaders.Clear();
    //config.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
}).AddHttpMessageHandler<AuthHttpMessageHandler>();

builder.Services.AddHttpClient<ShoppingService>(config =>
{
    config.BaseAddress = new Uri(builder.Configuration["Services:ShoppingCart:BaseUrl"].ToString());
    config.Timeout = TimeSpan.FromSeconds(10000);
    //config.DefaultRequestHeaders.Clear();
    //config.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
}).AddHttpMessageHandler<AuthHttpMessageHandler>();

builder.Services.AddHttpClient<ProductManagerService>(config =>
{
    config.BaseAddress = new Uri(builder.Configuration["Services:ProductsManager:BaseUrl"].ToString());
    config.Timeout = TimeSpan.FromSeconds(10000);
}).AddHttpMessageHandler<AuthHttpMessageHandler>();

builder.Services.AddHttpClient<CustomerService>(config =>
{
    config.BaseAddress = new Uri(builder.Configuration["Services:Customer:BaseUrl"].ToString());
    config.Timeout = TimeSpan.FromSeconds(10000);

}).AddHttpMessageHandler<AuthHttpMessageHandler>();

builder.Services.AddHttpClient<OrderService>(config =>
{
    config.BaseAddress = new Uri(builder.Configuration["Services:Orders:BaseUrl"].ToString());
    config.Timeout = TimeSpan.FromSeconds(10000);

}).AddHttpMessageHandler<AuthHttpMessageHandler>();

#endregion

#region localStorage
builder.Services.AddScoped<BaseLocalStorage>();
builder.Services.AddScoped<ShoppingLocalStorage>();
builder.Services.AddScoped<TokenJwtLocalStorage>();
#endregion

#region localStorage
builder.Services.AddScoped<ShoppingSessionStorage>();
#endregion
#region autentication
//builder.Services.AddScoped<AuthenticationStateProvider, DummyAuthProvider>();
//builder.Services.AddScoped<AuthIDentityJwtProvider>();
//builder.Services.AddScoped<AuthenticationStateProvider, AuthIDentityJwtProvider>(provider => provider.GetRequiredService<AuthIDentityJwtProvider>());
//builder.Services.AddScoped<IAuthProvider, AuthIDentityJwtProvider>(provider => provider.GetRequiredService<AuthIDentityJwtProvider>());
#endregion
//builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
//                .CreateClient("ShoppingService"));
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
                .CreateClient("CustomerService"));

builder.Services.AddSingleton<ConfigPaymentEpayCo>(x => new ConfigPaymentEpayCo
{
    key = builder.Configuration.GetValue<string>("EpayCo:PublicKey"),
    test = builder.Configuration.GetValue<string>("EpayCo:IsTest").ToLower()
});

builder.Services.AddSingleton<PaymentEpayCo>(x => new PaymentEpayCo
{

    tax_base = "0",
    tax = "0",
    external = builder.Configuration.GetValue<string>("EpayCo:IsExternal").ToLower(),
    lang = builder.Configuration.GetValue<string>("EpayCo:lang").ToLower(),
    currency = builder.Configuration.GetValue<string>("EpayCo:currency").ToLower(),
    country = builder.Configuration.GetValue<string>("EpayCo:country").ToLower(),
    response = builder.Configuration.GetValue<string>("EpayCo:response"),
    methodsDisable = builder.Configuration.GetSection("EpayCo:methodsDisable").GetChildren().ToArray().Select(c => c.Value).ToArray(),
    autoclick = builder.Configuration.GetValue<string>("EpayCo:autoclick").ToLower(),
});

//builder.Services.AddAuthorizationCore();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredSessionStorage();
builder.Services.AddBlazoredToast();
builder.Services.AddMudServices();

builder.Services.AddOidcAuthentication(options =>
{
    builder.Configuration.Bind("Auth0", options.ProviderOptions);
    options.ProviderOptions.ResponseType = "code";
    options.ProviderOptions.ClientId = builder.Configuration["Auth0:ClientId"].ToString();
    options.ProviderOptions.Authority = builder.Configuration["Auth0:Authority"].ToString();
    options.ProviderOptions.AdditionalProviderParameters.Add("audience", builder.Configuration["Auth0:Audience"]);
}).AddAccountClaimsPrincipalFactory<ArrayClaimsPrincipalFactory<RemoteUserAccount>>();


await builder.Build().RunAsync();
