using Hubtel.Wallets.Api.Models;
using Hubtel.Wallets.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Hubtel.Wallets.Api.Controllers
{
    [ApiVersion("1.0")]
    [Authorize]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("wallets")]
        public async Task<IActionResult> GetUserWallets()
        {
            var phone = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.MobilePhone).Value;
            if (phone == null)
            {
                return Unauthorized(new ServiceResponse<object>
                {
                    Success = false,
                    Message = "Make sure you're logged in.",
                    Data = new { }
                });
            }

            var response = await _userService.GetUserWithWallets(phone);
            if (response == null)
                return BadRequest(new ServiceResponse<object>
                {
                    Success = false,
                    Message = "Something went wrong",
                    Data = new {}
                });
            return Ok(response);
        }
    }
}
