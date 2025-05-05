using Azure;
using System.Net.Http.Headers;
using Microsoft.Graph.Models;
using Newtonsoft.Json;
using BackendAPI.Models;
using Microsoft.Extensions.Options;

namespace BackendAPI.Services
{
    public class UserService : IUserService
    {
        private readonly AzureAdOptions _azureAdOptions;
        private readonly IAuthService _authService;
        public UserService(IOptions<AzureAdOptions> azureAdOptions, IAuthService authService)
        {
            _azureAdOptions = azureAdOptions.Value;
            _authService = authService;
        }
        public async Task<User?> GetUserInfo(string authCode)
        {
            string authToken = await _authService.ExchangeAuthCodeWithTokenAsync(authCode);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                var response = await client.GetAsync("https://graph.microsoft.com/v1.0/me");
                response.EnsureSuccessStatusCode();
                var resultContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<User>(resultContent);
            }
        }
    }
}
