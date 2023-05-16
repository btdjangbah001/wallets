namespace Hubtel.Wallets.Api.Models
{
    public class Wallet
    {
        public int Id { get; set; }
        public WalletType Type { get; set; }
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public User Owner { get; set; }
        // Could add a list of transactions here but not necessary for this demo
    }

    public enum WalletType
    {
        MobileMoney,
        Card
    }
}
