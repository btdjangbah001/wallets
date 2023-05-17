using System;

namespace Hubtel.Wallets.Api.Models
{
    public class Wallet
    {
        public int Id { get; set; }
        public WalletType Type { get; set; }
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public User Owner { get; set; }
        public DateTime CreatedAt { get; set; }
        public WalletScheme Scheme { get; set; }
    }

    public class WalletResponse
    {
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public User Owner { get; set; }
        public WalletType Type { get; set; }
        public WalletScheme WalletScheme { get; set; }
    }

    public class UserWallet
    {
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public WalletType Type { get; set; }
        public WalletScheme Scheme { get; set; }
    }

    public enum WalletType
    {
        MobileMoney,
        Card
    }

    public enum WalletScheme
    {
        MasterCard,
        Visa,
        MTN,
        Vodafone,
        AirtelTigo
    }

    public class AddWallet
    {
        public WalletType Type { get; set; }
        public string AccountNumber { get; set; }
        public WalletScheme Scheme { get; set; }
    }
}
