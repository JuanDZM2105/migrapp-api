// Helpers/Implementations/EmailHelper.cs
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using migrapp_api.Models;

namespace migrapp_api.Helpers
{
    public class EmailHelper : IEmailHelper
    {
        public Task SendEmailAsync(string to, string subject, string body)
        {
            Console.WriteLine($"[EMAIL] To: {to} | Subject: {subject} | Body: {body}");
            return Task.CompletedTask;
        }
    }

    public class SmsHelper : ISmsHelper
    {
        public Task SendSmsAsync(string toPhone, string message)
        {
            Console.WriteLine($"[SMS] To: {toPhone} | Message: {message}");
            return Task.CompletedTask;
        }
    }

    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly IConfiguration _configuration;

        public JwtTokenGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(string email, string userType, int userId)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim("role", userType),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
