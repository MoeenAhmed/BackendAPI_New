using Azure.Core;
using BackendAPI.Models;
using BackendAPI.Services;
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
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        public UsersController(IUserService userService, IAuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }

        [Authorize]
        [HttpGet("GetUserInfo")]
        public async Task<IActionResult> GetUserInfo(string authCode)
        {
            APIResponse<User?> response = new APIResponse<User?>();

            try
            {
                string authToken = await _authService.ExchangeAuthCodeWithTokenAsync(authCode);

                if (string.IsNullOrEmpty(authToken))
                    throw new Exception("Token exchange failed");

                var user = await _userService.GetUserInfo(authToken);

                return Ok(new APIResponse<User?>("Success", "User info fetched successfully.", user));
                
            }
            catch (Exception ex)
            {
                return BadRequest(new APIResponse<string>("Exception", "Exception occured while generating the token", ex.Message));
            }
        }
    }
}
