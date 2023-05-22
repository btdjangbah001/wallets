using Hubtel.Wallets.Api.Models;
using Hubtel.Wallets.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Hubtel.Wallets.Api.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegister request)
        {
            var response = await _authService.Register(request);
            switch (response.StatusCode)
            {
                case 400:
                    return BadRequest(response);
                case 409:
                    return Conflict(response);
            } 
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLogin request)
        {
            var response = await _authService.LogIn(request);
            switch (response.StatusCode)
            {
                case 400:
                    return BadRequest(response);
                case 404:
                    return NotFound(response);
            }
            return Ok(response);
        }
    }
}