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
    [Route("/api/v{version:apiVersion}[controller]")]
    public class WalletController : Controller
    {
        private readonly IWalletService _walletService;
        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        [HttpGet]
        public async Task<IActionResult> GetWallets()
        {
            var response = await _walletService.GetWallets();
            if (response == null)
                return BadRequest(new { message = "No wallets found or something went wrong" });
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetWalletById(int id)
        {
            var response = await _walletService.GetWalletById(id);
            if (response == null)
                return BadRequest(new { message = "Wallet does not exist or something went wrong" });
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> AddWallet(AddWallet request)
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
            var response = await _walletService.AddWallet(request, phone);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWalletById(int id)
        {
            await _walletService.DeleteWalletById(id);
            return Ok();
        }
    }
}
