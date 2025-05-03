using Azure.Core;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
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

        [HttpGet("GetUserInfo")]
        public async Task<IActionResult> GetUserInfo([FromQuery]string code)
        {
            if (string.IsNullOrEmpty(code))
                return new BadRequestObjectResult("Code is empty");

            string authToken = await ExchangeCodeConfidentialWithTokenAsync(code);
            
            if (string.IsNullOrEmpty(authToken))
                return new BadRequestObjectResult("Token is empty");

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                var response = await client.GetAsync("https://graph.microsoft.com/v1.0/me");

                if (response.IsSuccessStatusCode)
                {
                    return Ok(await response.Content.ReadAsStringAsync());
                }
                else
                {
                    return BadRequest(await response.Content.ReadAsStringAsync());
                }
            }


        }

        private async Task<string> ExchangeCodeConfidentialWithTokenAsync(string code)
        {
             var cca = ConfidentialClientApplicationBuilder.Create(_azureAdOptions.ClientId)
                .WithClientSecret(_azureAdOptions.ClientSecret)
                .WithRedirectUri("https://project.islamweb.net.qa/auth/callback")
                .WithAuthority(new Uri($"https://login.microsoftonline.com/{_azureAdOptions.TenantId}"))
                .Build();

            string[] scopes = new string[] { "User.Read" };

            var result = await cca.AcquireTokenByAuthorizationCode(scopes, code).ExecuteAsync();
            return result.AccessToken;
        }

        [HttpPost("exchange")]
        public async Task<IActionResult> ExchangeCodePublicWithTokenAsync([FromBody] AuthCodeDto dto)
        {
            var client = new HttpClient();

            var parameters = new Dictionary<string, string>
            {
                ["client_id"] = "b11f6189-4ab0-42a1-b12a-43a4c5b29dd4",
                ["grant_type"] = "authorization_code",
                ["code"] = dto.Code,
                ["redirect_uri"] = "https://project.islamweb.net.qa/auth/callback",
                ["code_verifier"] = dto.CodeVerifier,
                ["scope"] = "User.Read"
            };

            var response = await client.PostAsync(
                "https://login.microsoftonline.com/7753f32f-c409-43e1-aed4-3fe3eacc6dbf/oauth2/v2.0/token",
                new FormUrlEncodedContent(parameters)
            );

            var responseContent = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                return BadRequest(responseContent);

            return Ok(responseContent); // Send tokens to frontend (or keep them server-side)
        }

    }

    public class AzureAdOptions
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUri { get; set; }
        public string TenantId { get; set; }

    }

    public class AuthCodeDto
    {
        public string Code { get; set; }
        public string CodeVerifier { get; set; }
    }
}
