using BackendAPI.Models;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;

namespace BackendAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly AzureAdOptions _azureAdOptions;
        public AuthService(IOptions<AzureAdOptions> azureAdOptions)
        {
            _azureAdOptions = azureAdOptions.Value;
        }

        public async Task<string> ExchangeAuthCodeWithTokenAsync(string authCode)
        {
            var cca = ConfidentialClientApplicationBuilder.Create(_azureAdOptions.ClientId)
                   .WithClientSecret(_azureAdOptions.ClientSecret)
                   .WithRedirectUri(_azureAdOptions.CallbackUrl)
                   .WithAuthority(new Uri($"https://login.microsoftonline.com/{_azureAdOptions.TenantId}"))
                   .Build();

            var authenticationResult = await cca.AcquireTokenByAuthorizationCode(_azureAdOptions.Scopes, authCode).ExecuteAsync();
            return authenticationResult.AccessToken;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            using (var client = new HttpClient())
            {
                var parameters = new Dictionary<string, string>
                {
                    ["client_id"] = $"{_azureAdOptions.ClientId}",
                    ["client_secret"] = $"{_azureAdOptions.ClientSecret}",
                    ["grant_type"] = "client_credentials",
                    ["resource"] = $"{_azureAdOptions.ClientId}"
                };

                var result = await client.PostAsync($"https://login.microsoftonline.com/{_azureAdOptions.TenantId}/oauth2/token", new FormUrlEncodedContent(parameters));

                result.EnsureSuccessStatusCode();

                var responseBody = await result.Content.ReadAsStringAsync();

                var jsonResponse = JObject.Parse(responseBody);

                return jsonResponse["access_token"].ToString();

            }
        }
    }
}
