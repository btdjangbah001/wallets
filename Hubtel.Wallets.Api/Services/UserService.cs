using Hubtel.Wallets.Api.Data;
using Hubtel.Wallets.Api.Models;
using Hubtel.Wallets.Api.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Hubtel.Wallets.Api.Services
{
    public interface IUserService
    {
        Task<ServiceResponse<User>> GetUserById(string phone);
        Task<ServiceResponse<UserResponse>> GetUserWithWallets(string phone);
        Task<int> CountUserWallets(string phone);
    }

    public class UserService: IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ServiceResponse<User>> GetUserById(string phone)
        {
            return new ServiceResponse<User>
            {
                StatusCode = 200,
                Success = true,
                Message = "User retrieved successfully",
                Data = await _userRepository.GetUserById(phone),
            };
        }

        public async Task<ServiceResponse<UserResponse>> GetUserWithWallets(string phone)
        {
            return new ServiceResponse<UserResponse>
            {
                StatusCode = 200,
                Success = true,
                Message = "User retrieved successfully",
                Data = await _userRepository.GetUserWithWallets(phone),
            };
        }

        public async Task<int> CountUserWallets(string phone)
        {
            return await _userRepository.CountUserWallets(phone);
        }
    }
}
