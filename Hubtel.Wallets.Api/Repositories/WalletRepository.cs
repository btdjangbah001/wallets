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
        Task<Wallet> AddWallet(Wallet wallet);
        Task<WalletResponse> GetWalletById(int id);
        Task<List<WalletResponse>> GetWalletsByUserId(string phoneNumber);
        Task<bool> CheckWalletExists(Wallet wallet);
        Task<List<OthersWallet>> GetWallets();
        Task<Wallet> DeleteWalletById(int id);
        Task<string> GetWalletOwner(int id);
    }
    public class WalletRepository: IWalletRepository
    {
        private readonly DataContext _context;
        public WalletRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<Wallet> AddWallet(Wallet wallet)
        {
            await _context.Wallets.AddAsync(wallet);
            await _context.SaveChangesAsync();

            return wallet;
        }

        public async Task<WalletResponse> GetWalletById(int id)
        {
            var wallet = await _context.Wallets.Include(u => u.Owner).FirstOrDefaultAsync(w => w.Id == id);

            return new WalletResponse
            {
                Id = wallet.Id,
                AccountNumber = wallet.AccountNumber,
                Balance = wallet.Balance,
                Type = wallet.Type,
                Scheme = wallet.Scheme,
                CreatedAt = wallet.CreatedAt,
            };
        }

        public async Task<string> GetWalletOwner(int id)
        {
            var wallet = await _context.Wallets.FindAsync(id);
            return wallet.Owner.PhoneNumber;
        }

        public async Task<List<WalletResponse>> GetWalletsByUserId(string phoneNumber)
        {
            var wallets = await _context.Wallets
                .Where(w => w.Owner.PhoneNumber == phoneNumber)
                .ToListAsync();

            var walletResponses = await Task.WhenAll(wallets.Select(async w => new WalletResponse
            {
                Id = w.Id,
                AccountNumber = w.AccountNumber,
                Balance = w.Balance,
                Type = w.Type, 
                Scheme = w.Scheme,
                CreatedAt = w.CreatedAt,
            }));

            return walletResponses.ToList();
        }

        public async Task<bool> CheckWalletExists(Wallet wallet)
        {
            return await _context.Wallets.FirstOrDefaultAsync(w => w.AccountNumber == wallet.AccountNumber && w.Type == wallet.Type && w.Scheme == wallet.Scheme) != null;
        }

        public async Task<List<OthersWallet>> GetWallets()
        {
            return await _context.Wallets.Select(w => new OthersWallet
            {
                Id = w.Id,
                AccountNumber = w.AccountNumber,
                Balance = w.Balance,
                Type = w.Type,
                Owner = w.Owner.Username,
                PhoneNumber = w.Owner.PhoneNumber,
            }).ToListAsync();
        }

        public async Task<Wallet> DeleteWalletById(int id)
        {
            var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.Id == id);
            _context.Wallets.Remove(wallet);
            await _context.SaveChangesAsync();

            return wallet;
        }
    }
}
