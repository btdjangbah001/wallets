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
using System.Text.RegularExpressions;
using Hubtel.Wallets.Api.Utils;

namespace Hubtel.Wallets.Api.Services
{
    public interface IAuthService
    {
        Task<ServiceResponse<string>> Register(UserRegister userReg);
        Task<ServiceResponse<string>> LogIn(UserLogin userLogin);
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
     
        public async Task<ServiceResponse<string>> Register(UserRegister userReg)
        {
            if (!userReg.Password.Equals(userReg.ConfirmPassword))
                return new ServiceResponse<string>
                {
                    StatusCode = 400,
                    Success = false,
                    Message = "Passwords do not match!",
                    Data = String.Empty
                };

            if (!IsStrongPassword(userReg.Password))
                return new ServiceResponse<string>
                {
                    StatusCode = 400,
                    Success = false,
                    Message = "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one number and one special character",
                    Data = String.Empty
                };

            if (!Utilities.IsValidPhoneNumber(userReg.PhoneNumber))
                return new ServiceResponse<string>
                {

                    StatusCode = 400,
                    Success = false,
                    Message = "Please provide a valid number.",
                    Data = String.Empty
                };

            if (await UserExists(userReg.PhoneNumber))
                return new ServiceResponse<string>
                {
                    StatusCode = 409,
                    Success = false,
                    Message = "User with phone number " + userReg.PhoneNumber + " already exists",
                    Data = String.Empty
                };

            var user = new User
            {
                PhoneNumber = userReg.PhoneNumber,
                Username = userReg.Username,
                Role = "User",
            };

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(userReg.Password, out passwordHash, out passwordSalt);
     
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
     
            await _userRepository.AddUser(user);
            
            var token = CreateToken(user.PhoneNumber, user.Role);

            return new ServiceResponse<string>
            {
                StatusCode = 200,
                Success = true,
                Message = "User created successfully",
                Data = token,
            };
        }

        public async Task<ServiceResponse<string>> LogIn(UserLogin userLogin)
        {
            var user = await _userRepository.GetUserById(userLogin.PhoneNumber);
            if (user == null)
                return new ServiceResponse<string>
                {
                    StatusCode = 404,
                    Success = false,
                    Message = "Account does not exists with phone number " + userLogin.PhoneNumber,
                    Data = String.Empty
                };

            if (!VerifyPasswordHash(userLogin.Password, user.PasswordHash, user.PasswordSalt))
                return new ServiceResponse<string>
                {
                    StatusCode = 400,
                    Success = false,
                    Message = "Your pssword is incorrect!",
                    Data = String.Empty
                };

            var token = CreateToken(user.PhoneNumber, user.Role);

            return new ServiceResponse<string>
            {
                StatusCode = 200,
                Success = true,
                Message = "Logged in successfully",
                Data = token,
            };
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

        private string CreateToken(string phone, string role)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.MobilePhone, phone),
                new Claim(ClaimTypes.Role, role)
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

        public static bool IsStrongPassword(string password)
        {
            // Define the regular expression for password strength requirements
            var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).{8,}$");

            // Check if the password matches the regular expression pattern
            return regex.IsMatch(password);
        }
    }
}
