using Hubtel.Wallets.Api.Data;
using Hubtel.Wallets.Api.Models;
using Hubtel.Wallets.Api.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hubtel.Wallets.Api.Services
{
    public interface IWalletService
    {
        Task<ServiceResponse<object>> AddWallet(AddWallet wallet, string phone);
        Task<ServiceResponse<List<Wallet>>> GetWallets();
        Task<ServiceResponse<WalletResponse>> GetWalletById(int id);
        Task<ServiceResponse<List<WalletResponse>>> GetWalletsByUserPhone(string phone);
        Task DeleteWalletById(int id);
    }
    public class WalletService: IWalletService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly IUserService _userService;
        public WalletService(IWalletRepository walletRepository, IUserService userService)
        {
            _walletRepository = walletRepository;
            _userService = userService;
        }

        public async Task<ServiceResponse<object>> AddWallet(AddWallet addWallet, string phone)
        {
            // I could write logic here to make sure cards are not mobile money schemes and vice versa
            var wallet = new Wallet
            {
                AccountNumber = addWallet.AccountNumber,
                Owner = await _userService.GetUserById(phone),
                Type = addWallet.Type,
                Scheme = addWallet.Scheme,
                Balance = 0,
                CreatedAt = System.DateTime.UtcNow,
            };

            if (wallet.Type == WalletType.Card)
            {
                wallet.AccountNumber = wallet.AccountNumber.Substring(0, 6);
            }

            if (await _userService.CountUserWallets(wallet.Owner.PhoneNumber) >= 5)
            {
                return new ServiceResponse<object>
                {
                    Success = false,
                    Message = "User cannot have more than 5 wallets",
                    Data = new { }
                };
            }
            var oldWallet = await _walletRepository.GetWalletByWalletNumber(wallet.AccountNumber);
            if (oldWallet != null)
            {
                return new ServiceResponse<object>
                {
                    Success = false,
                    Message = "Wallet already exists",
                    Data = new { }
                };
            }
            await _walletRepository.AddWallet(wallet);
            return new ServiceResponse<object>
            {
                Success = true,
                Message = "Wallet added successfully",
                Data = new { }
            };
        }

        public async Task<ServiceResponse<List<Wallet>>> GetWallets()
        {
            var wallets = await _walletRepository.GetWallets();
            return new ServiceResponse<List<Wallet>>
            {
                Success = true,
                Message = "Wallets retrieved successfully",
                Data = wallets
            };
        }

        public async Task<ServiceResponse<WalletResponse>> GetWalletById(int id)
        {
            var wallet = await _walletRepository.GetWalletById(id);
            // i am currently sending too much owner information to client I could clean up a bit 
            return new ServiceResponse<WalletResponse>
            {
                Success = true,
                Message = "Data retrieved successfully",
                Data = wallet
            };
        }

        public async Task<ServiceResponse<List<WalletResponse>>> GetWalletsByUserPhone(string phone)
        {
            var wallets = await _walletRepository.GetWalletsByUserId(phone);
            return new ServiceResponse<List<WalletResponse>>
            {
                Success = true,
                Message = "Data retrieved successfully",
                Data = wallets
            };
        }

        public async Task DeleteWalletById(int id)
        {
            await _walletRepository.DeleteWalletById(id);
        }
    }
}
