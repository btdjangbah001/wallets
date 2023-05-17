using Hubtel.Wallets.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Hubtel.Wallets.Api.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
    }
}
