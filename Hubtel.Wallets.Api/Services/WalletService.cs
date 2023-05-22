using Hubtel.Wallets.Api.Data;
using Hubtel.Wallets.Api.Models;
using Hubtel.Wallets.Api.Repositories;
using Hubtel.Wallets.Api.Utils;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Hubtel.Wallets.Api.Services
{
    public interface IWalletService
    {
        Task<ServiceResponse<WalletResponse>> AddWallet(AddWallet wallet, string phone);
        Task<ServiceResponse<List<OthersWallet>>> GetWallets();
        Task<ServiceResponse<WalletResponse>> GetWalletById(int id);
        Task<ServiceResponse<List<WalletResponse>>> GetWalletsByUserPhone(string phone);
        Task<ServiceResponse<WalletResponse>> DeleteWalletById(int id, string phone, string role);
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

        public async Task<ServiceResponse<WalletResponse>> AddWallet(AddWallet addWallet, string phone)
        {
            var validation = ValidateWallet(addWallet);

            if (!validation.Success) return validation;

            var user = await _userService.GetUserById(phone);
            var wallet = new Wallet
            {
                AccountNumber = addWallet.AccountNumber,
                Owner = user.Data,
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
                return new ServiceResponse<WalletResponse>
                {
                    StatusCode = 400,
                    Success = false,
                    Message = "User cannot have more than 5 wallets",
                    Data = new WalletResponse 
                    {
                        AccountNumber = addWallet.AccountNumber,
                        Type = addWallet.Type,
                        Scheme = addWallet.Scheme,
                    }
                };
            }
            
            if (await _walletRepository.CheckWalletExists(wallet))
            {
                return new ServiceResponse<WalletResponse>
                {
                    StatusCode = 409,
                    Success = false,
                    Message = "Wallet with account number " + addWallet.AccountNumber + " already exists",
                    Data = new WalletResponse 
                    {
                        AccountNumber = addWallet.AccountNumber,
                        Type = addWallet.Type,
                        Scheme = addWallet.Scheme,
                    }
                };
            }
            var data = await _walletRepository.AddWallet(wallet);
            return new ServiceResponse<WalletResponse>
            {
                StatusCode = 201,
                Success = true,
                Message = "Wallet added successfully",
                Data = new WalletResponse 
                {
                    Id = data.Id,
                    AccountNumber = data.AccountNumber,
                    Balance = data.Balance,
                    Type = data.Type,
                    Scheme = data.Scheme,
                    CreatedAt = data.CreatedAt,
                }
            };
        }

        public async Task<ServiceResponse<List<OthersWallet>>> GetWallets()
        {
            var wallets = await _walletRepository.GetWallets();
            return new ServiceResponse<List<OthersWallet>>
            {
                StatusCode = 200,
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
                StatusCode = 200,
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
                StatusCode = 200,
                Success = true,
                Message = "Data retrieved successfully",
                Data = wallets
            };
        }

        public async Task<ServiceResponse<WalletResponse>> DeleteWalletById(int id, string phone, string role)
        {
            // Users should not be able to delete wallets with balance greater than 0
            // but not part of the requirements and we're not dealing with balances

            if (role != "Admin" || phone != await _walletRepository.GetWalletOwner(id))
                return new ServiceResponse<WalletResponse>
                {
                    StatusCode = 403,
                    Success = false,
                    Message = "You are not allowed to perform this action",
                    Data = new WalletResponse { }
                };

            var data = await _walletRepository.DeleteWalletById(id);

            return new ServiceResponse<WalletResponse>
            {
                StatusCode = 200,
                Success = true,
                Message = "",
                Data = new WalletResponse
                {
                    Id = data.Id,
                    AccountNumber = data.AccountNumber,
                    Balance = data.Balance,
                    CreatedAt = data.CreatedAt,
                    Type = data.Type,
                    Scheme = data.Scheme,
                }
            };
        }

        private ServiceResponse<WalletResponse> ValidateWallet(AddWallet wallet)
        {
            if ((wallet.Type == WalletType.Card && !Utilities.IsValidCreditCardNumber(wallet.AccountNumber)) ||
                (wallet.Type == WalletType.MobileMoney && !Utilities.IsValidPhoneNumber(wallet.AccountNumber))
                )
            {
                return new ServiceResponse<WalletResponse>
                {
                    StatusCode = 400,
                    Success = false,
                    Message = "Invalid account number",
                    Data = new WalletResponse
                    {
                        AccountNumber = wallet.AccountNumber,
                        Type = wallet.Type,
                        Scheme = wallet.Scheme,
                    }
                };
            }

            switch (wallet.Type)
            {
                case WalletType.Card:
                    if (wallet.Scheme == WalletScheme.MasterCard || wallet.Scheme == WalletScheme.Visa)
                    {
                        return new ServiceResponse<WalletResponse>
                        {
                            Success = true,
                            Message = string.Empty,
                        };
                    }
                    else
                    {
                        return new ServiceResponse<WalletResponse>
                        {
                            StatusCode = 400,
                            Success = false,
                            Message = "Card can only be MasterCard or Visa",
                            Data = new WalletResponse
                            {
                                AccountNumber = wallet.AccountNumber,
                                Type = wallet.Type,
                                Scheme = wallet.Scheme,
                            }
                        };
                    }

                case WalletType.MobileMoney:
                    if (wallet.Scheme == WalletScheme.MasterCard || wallet.Scheme == WalletScheme.Visa)
                    {
                        return new ServiceResponse<WalletResponse>
                        {
                            StatusCode = 400,
                            Success = false,
                            Message = "Mobile money can only be Vodafone, Mtn or AirtelTigo",
                            Data = new WalletResponse 
                            {
                                AccountNumber = wallet.AccountNumber,
                                Type = wallet.Type,
                                Scheme = wallet.Scheme,
                            }
                        };
                    }
                    else
                    {
                        return new ServiceResponse<WalletResponse>
                        {
                            Success = true,
                            Message = string.Empty
                        };
                    }
            }
            // control should never reach here
            return new ServiceResponse<WalletResponse> { Success = false };
        }
    }
}
