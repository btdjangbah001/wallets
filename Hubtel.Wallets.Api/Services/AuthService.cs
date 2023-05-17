using Hubtel.Wallets.Api.Data;
using Hubtel.Wallets.Api.Models;
using Hubtel.Wallets.Api.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;
using System;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Hubtel.Wallets.Api.Services
{
    public interface IAuthService
    {
        Task<string> Register(UserRegister userReg);
        Task<string> LogIn(UserLogin userLogin);
    }

    public class AuthService: IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }
     
        public async Task<string> Register(UserRegister userReg)
        {
            if (await UserExists(userReg.PhoneNumber))
                return null;

            var user = new User
            {
                PhoneNumber = userReg.PhoneNumber,
                Username = userReg.Username
            };

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(userReg.Password, out passwordHash, out passwordSalt);
     
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
     
            await _userRepository.AddUser(user);
     
            return CreateToken(user.PhoneNumber);
        }

        public async Task<string> LogIn(UserLogin userLogin)
        {
            var user = await _userRepository.GetUserById(userLogin.PhoneNumber);
            if (user == null)
                return null;
            if (!VerifyPasswordHash(userLogin.Password, user.PasswordHash, user.PasswordSalt)) return null;

            return CreateToken(user.PhoneNumber);
        }

        private async Task<bool> UserExists(string phone)
        {
            if (await _userRepository.GetUserById(phone) != null)
                return true;
            return false;
        }
        
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private string CreateToken(string phone)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.MobilePhone, phone),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            SigningCredentials cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = cred
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public string GenerateJwtToken(string phone)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var now = DateTime.UtcNow;

            var claims = new[]
            {
                new Claim(ClaimTypes.MobilePhone, phone)
            };

            var token = new JwtSecurityToken(
                claims: claims,
                notBefore: now,
                expires: now.AddDays(1),
                signingCredentials: credentials
            );

            var encodedToken = new JwtSecurityTokenHandler().WriteToken(token);
            return encodedToken;
        }

        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])
                    {
                        return false;
                    }
                }
                return true;
            }
        }
    }
}
