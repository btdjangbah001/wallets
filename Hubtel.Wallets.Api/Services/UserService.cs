using Hubtel.Wallets.Api.Data;
using Hubtel.Wallets.Api.Models;
using Hubtel.Wallets.Api.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Hubtel.Wallets.Api.Services
{
    public interface IUserService
    {
        Task<User> GetUserById(string phone);
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

        public async Task<User> GetUserById(string phone)
        {
            return await _userRepository.GetUserById(phone);
        }

        public async Task<ServiceResponse<UserResponse>> GetUserWithWallets(string phone)
        {
            var res = await _userRepository.GetUserWithWallets(phone);
            return new ServiceResponse<UserResponse>
            {
                Success = true,
                Message = "User retrieved successfully",
                Data = res,
            };
        }

        public async Task<int> CountUserWallets(string phone)
        {
            return await _userRepository.CountUserWallets(phone);
        }
    }
}
