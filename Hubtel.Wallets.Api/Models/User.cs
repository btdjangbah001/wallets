using System.Collections;
using System.Collections.Generic;

namespace Hubtel.Wallets.Api.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Wallet> Wallets { get; set; }
    }
}
