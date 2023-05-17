using Hubtel.Wallets.Api.Data;
using Hubtel.Wallets.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.Wallets.Api.Repositories
{
    public interface IUserRepository
    {
        Task AddUser(User user);
        Task<User> GetUserByUsername(string username);
        Task<User> GetUserById(string phone);
        Task<UserResponse> GetUserWithWallets(string phone);
        Task<int> CountUserWallets(string phone);
    }
    public class UserRepository: IUserRepository
    {
        private readonly DataContext _context;
        public UserRepository(DataContext context)
        {
            _context = context;
        }

        public async Task AddUser(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User> GetUserByUsername(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User> GetUserById(string phone)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phone);
        }

        public async Task<UserResponse> GetUserWithWallets(string phone)
        {
            var user = await _context.Users
                .Include(u => u.Wallets)
                .FirstOrDefaultAsync(u => u.PhoneNumber == phone);
            return new UserResponse
            {
                Username = user.Username,
                Wallets = user.Wallets.Select(w => new UserWallet
                {
                    AccountNumber = w.AccountNumber,
                    Balance = w.Balance,
                    Type = w.Type,
                }).ToList(),
            };
        }

        public async Task<int> CountUserWallets(string phone)
        {
            return await _context.Wallets.CountAsync(w => w.Owner.PhoneNumber == phone);
        }
    }
}
