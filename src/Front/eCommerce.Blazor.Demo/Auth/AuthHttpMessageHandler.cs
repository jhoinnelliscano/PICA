using eCommerce.Blazor.Demo.LocalStorage;
using eCommerce.Commons.Objects.Requests.Authorizer;
using eCommerce.Commons.Objects.Responses;
using eCommerce.Commons.Objects.Responses.Authorizer;
using eCommerce.Commons.Utilities;
using eCommerce.Services.Exceptions;
using eCommerce.Services.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace eCommerce.Blazor.Demo.Auth
{
    public class AuthHttpMessageHandler : DelegatingHandler
    {
        private TokenJwtLocalStorage _localStorage;
        private IAccessTokenProvider _tokenProvider;
        private IConfiguration _configuration;

        public AuthHttpMessageHandler(TokenJwtLocalStorage localStorage, IAccessTokenProvider TokenProvider, IConfiguration configuration)
        {
            _localStorage = localStorage;
            _tokenProvider = TokenProvider;
            _configuration = configuration;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.RequestUri.AbsolutePath.Contains("/Authorizer/"))
            {
                return await base.SendAsync(request, cancellationToken);
            }

            var token = "";
            if (request.RequestUri.AbsolutePath.Contains("/Product/"))
            {
                token = await GetMachineAccessToken();
            }
            else 
            {
                var accessTokenResult = await _tokenProvider.RequestAccessToken();
                if (accessTokenResult.TryGetToken(out var accessToken))
                {
                    var accessTokenVale = accessToken.Value;
                    token = accessTokenVale;
                }
            }

            if (string.IsNullOrEmpty(token))
            {
                return await base.SendAsync(request, cancellationToken);
            }

            request.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);
            return await base.SendAsync(request, cancellationToken);
        }

        private async Task<string> GetMachineAccessToken() 
        {
            await ValidateMachineAccessToken();
            var accessToken = await _localStorage.GetToken();
            return accessToken.Access_token;        
        }

        private async Task ValidateMachineAccessToken()
        {
            try
            {
                var accessToken = await _localStorage.GetToken();

                //if (accessToken == null || (accessToken != null && accessToken.Expires_at <= DateTime.Now))
                //{
                    using var client = new HttpClient();
                    client.BaseAddress = new Uri(_configuration["Auth0:Machine:Authority"].ToString());
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    string clientId = _configuration["Auth0:Machine:ClientId"].ToString();
                    string clientSecrect = _configuration["Auth0:Machine:ClientSecrect"].ToString();
                    AccessTokenRequest accessTokenReq = new(clientId, clientSecrect, _configuration["Auth0:Audiences:ProductsAud"].ToString());

                    var response = await client.PostAsJsonAsync(_configuration["Auth0:Machine:Method"].ToString(), accessTokenReq);
                    if (response.IsSuccessStatusCode)
                    {
                        var newAccessToken = JsonSerializer.Deserialize<AccessTokenResponse>(await response.Content.ReadAsStringAsync(), JsonUtilities.jsonSettings);
                        await _localStorage.RemoveToken();
                        await _localStorage.SaveToken(newAccessToken);
                    }
                    else
                    {
                        var error = JsonSerializer.Deserialize<ServiceResponseError>(await response.Content.ReadAsStringAsync(), JsonUtilities.jsonSettings);
                        throw new ServiceException(error, 1);
                    }
                //}
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
