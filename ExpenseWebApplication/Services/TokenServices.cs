using System;
using System.Security.Cryptography;
using ExpenseWebApplication.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Serilog;
using System.Configuration;
using System.Security.Claims;

namespace ExpenseWebApplication.Services
{
    public class TokenService
    {
        private readonly string _jwtSecret;

        public TokenService()
        {
            _jwtSecret = ConfigurationManager.AppSettings["JwtSecret"];
        }
        public Tokens GenerateAccessToken(Users user)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSecret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim("Age", user.Age.ToString()),
                        new Claim("Gender", user.Gender),
                        new Claim("Department", user.Department),
                        new Claim("role", user.RoleId.ToString())
            }),
                    Expires = DateTime.UtcNow.AddMinutes(30),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                return new Tokens
                {
                    UserId = user.Id,
                    Token = tokenString,
                    JwtId = Guid.NewGuid().ToString(),
                    IsRevoked = false,
                    DateAdded = DateTime.Now,
                    DateExpire = DateTime.UtcNow.AddMinutes(30)
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error generating access token for user {UserId}", user.Id);
                throw;
            }
        }
        public Tokens GenerateRefreshToken(Users user)
        {
            try
            {
                var refreshToken = GenerateSecureToken();
                return new Tokens
                {
                    UserId = user.Id,
                    Token = refreshToken,
                    JwtId = Guid.NewGuid().ToString(),
                    IsRevoked = false,
                    DateAdded = DateTime.UtcNow,
                    DateExpire = DateTime.UtcNow.AddDays(7)
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to generate refresh token for user {UserId}", user.Id);
                throw;
            }
        }
        private string GenerateSecureToken()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] tokenData = new byte[32];
                rng.GetBytes(tokenData);
                return Convert.ToBase64String(tokenData);
            }
        }
    }
}