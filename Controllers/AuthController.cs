using BackendAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;

namespace BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AzureAdOptions _azureAdOptions;
        public AuthController(IOptions<AzureAdOptions> azureAdOptions)
        {
            _azureAdOptions = azureAdOptions.Value;
        }

        [HttpGet("GetAccessToken")]
        public async Task<IActionResult> GetAccessTokenAsync()
        {
            APIResponse<string> response = new APIResponse<string>();

            try
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
                    
                    var accessToken = jsonResponse["access_token"].ToString();

                    return Ok(new APIResponse<string>("Success", "Token generated successfully.", accessToken));
                };
            }
            catch (Exception ex)
            {
                return BadRequest(new APIResponse<string>("Failed", "Exception occured while generating the token", ex.Message));
            }
        }
    }
}
