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
            if (response == null)
                return BadRequest(new { message = "User already exists or something went wrong" });
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLogin request)
        {
            var response = await _authService.LogIn(request);
            if (response == null)
                return NotFound(new { message = "Invalid credentials or soemthing went wrong" });
            return Ok(response);
        }
    }
}