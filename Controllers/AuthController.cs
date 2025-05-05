using BackendAPI.Models;
using BackendAPI.Services;
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
        private readonly IAuthService _authService;
        public AuthController(IOptions<AzureAdOptions> azureAdOptions, IAuthService authService)
        {
            _azureAdOptions = azureAdOptions.Value;
            _authService = authService;
        }

        [HttpGet("GetAccessToken")]
        public async Task<IActionResult> GetAccessTokenAsync()
        {
            APIResponse<string> response = new APIResponse<string>();

            try
            {
                var accessToken = await _authService.GetAccessTokenAsync();
                return Ok(new APIResponse<string>("Success", "Token generated successfully.", accessToken));
            }
            catch (Exception ex)
            {
                return BadRequest(new APIResponse<string>("Exception", "Exception occured while generating the token", ex.Message));
            }
        }

    }
}
