using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hubtel.Wallets.Api.Models
{
    public class User
    {
        [Required]
        [Key]
        public string PhoneNumber { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public byte[] PasswordHash { get; set; }
        [Required]
        public byte[] PasswordSalt { get; set; }
        public List<Wallet> Wallets { get; set; }
        public string Role { get; set; }
    }

    public class UserResponse
    {
        public string Username { get; set; }
        public List<UserWallet> Wallets { get; set; }
    }

    public class UserLogin
    {
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Password { get; set; }
    }

    public class UserRegister
    {
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
    }
}
