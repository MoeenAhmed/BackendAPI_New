using Azure.Core;
using BackendAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Graph.Models;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace BackendAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AzureAdOptions _azureAdOptions;
        public UsersController(IOptions<AzureAdOptions> azureAdOptions)
        {
            _azureAdOptions = azureAdOptions.Value;
        }

        [Authorize]
        [HttpGet("GetUserInfo")]
        public async Task<IActionResult> GetUserInfo(string authCode)
        {
            APIResponse<object> response = new APIResponse<object>();

            try
            {
                string authToken = await ExchangeAuthCodeWithTokenAsync(authCode);

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                    var result = await client.GetAsync("https://graph.microsoft.com/v1.0/me");

                    if (result.IsSuccessStatusCode)
                    {
                        response.Status = "Success";
                        response.Message = "User info fetched successfully.";
                        response.Data = JsonConvert.DeserializeObject<User>(await result.Content.ReadAsStringAsync());
                        return Ok(response);
                    }
                    else
                    {
                        response.Status = "Failure";
                        response.Message = "User info fetching failed";
                        response.Data = await result.Content.ReadAsStringAsync();
                        return BadRequest(response);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Status = "Exception";
                response.Message = $"{ex.Message}";
                response.Data = null;
                return BadRequest(response);
            }
        }

        private async Task<string> ExchangeAuthCodeWithTokenAsync(string authCode)
        {
            var cca = ConfidentialClientApplicationBuilder.Create(_azureAdOptions.ClientId)
                   .WithClientSecret(_azureAdOptions.ClientSecret)
                   .WithRedirectUri(_azureAdOptions.CallbackUrl)
                   .WithAuthority(new Uri($"https://login.microsoftonline.com/{_azureAdOptions.TenantId}"))
                   .Build();

            var authenticationResult = await cca.AcquireTokenByAuthorizationCode(_azureAdOptions.Scopes, authCode).ExecuteAsync();
            return authenticationResult.AccessToken;
        }
    }
}
