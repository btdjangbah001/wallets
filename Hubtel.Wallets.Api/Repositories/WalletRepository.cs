using Hubtel.Wallets.Api.Data;
using Hubtel.Wallets.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.Wallets.Api.Repositories
{
    public interface IWalletRepository
    {
        Task AddWallet(Wallet wallet);
        Task<WalletResponse> GetWalletById(int id);
        Task<List<WalletResponse>> GetWalletsByUserId(string phoneNumber);
        Task<Wallet> GetWalletByWalletNumber(string walletNumber);
        Task<List<Wallet>> GetWalletByType(WalletType type);
        Task<List<Wallet>> GetWallets();
        Task DeleteWalletById(int id);
    }
    public class WalletRepository: IWalletRepository
    {
        private readonly DataContext _context;
        public WalletRepository(DataContext context)
        {
            _context = context;
        }

        public async Task AddWallet(Wallet wallet)
        {
            await _context.Wallets.AddAsync(wallet);
            await _context.SaveChangesAsync();
        }

        public async Task<WalletResponse> GetWalletById(int id)
        {
            var wallet = await _context.Wallets.Include(u => u.Owner).FirstOrDefaultAsync(w => w.Id == id);

            return new WalletResponse
            {
                AccountNumber = wallet.AccountNumber,
                Balance = wallet.Balance,
                Type = wallet.Type,
                Owner = wallet.Owner,
            };
        }

        public async Task<List<WalletResponse>> GetWalletsByUserId(string phoneNumber)
        {
            var wallets = await _context.Wallets
                .Where(w => w.Owner.PhoneNumber == phoneNumber)
                .ToListAsync();

            var walletResponses = await Task.WhenAll(wallets.Select(async w => new WalletResponse
            {
                AccountNumber = w.AccountNumber,
                Balance = w.Balance,
                Type = w.Type,
                Owner = await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber)
            }));

            return walletResponses.ToList();
        }

        public async Task<Wallet> GetWalletByWalletNumber(string walletNumber)
        {
            return await _context.Wallets.FirstOrDefaultAsync(w => w.AccountNumber == walletNumber);
        }

        public async Task<List<Wallet>> GetWalletByType(WalletType type)
        {
            return await _context.Wallets.Where(w => w.Type == type).ToListAsync();
        }

        public async Task<List<Wallet>> GetWallets()
        {
            return await _context.Wallets.ToListAsync();
        }

        public async Task DeleteWalletById(int id)
        {
            var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.Id == id);
            _context.Wallets.Remove(wallet);
            await _context.SaveChangesAsync();
        }
    }
}
