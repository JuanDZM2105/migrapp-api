// Helpers/Implementations/EmailHelper.cs
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using migrapp_api.Models;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace migrapp_api.Helpers
{
    public class EmailHelper : IEmailHelper
    {
        private readonly string _sendGridApiKey = "apikey";
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var client = new SendGridClient(_sendGridApiKey);
            var from = new EmailAddress("juandzm2105@gmail.com", "Tu App");
            var toEmail = new EmailAddress(to);
            string emailBody = $@"
                <html>
                    <body>
                        <div style='text-align: center; font-family: Arial, sans-serif;'>
                            <h2 style='font-size: 20px;'>Hola</h2>
                            <p style='font-size: 16px;'>Tu código de verificación de MigrApp es:</p>
                            <h3 style='font-size: 48px; font-weight: bold; color: #0D47A1; margin-top: 20px;'>
                                {body}
                            </h3> <!-- Código de verificación grande y separado -->
                            <p style='font-size: 16px; margin-top: 30px;'>
                                Para continuar con tu inicio de sesión, regresa a la aplicación e ingresa el código de verificación.
                                Este código expirará en 5 minutos.
                            </p>
                        </div>
                    </body>
                </html>
                ";
            var msg = MailHelper.CreateSingleEmail(from, toEmail, subject, "", emailBody);

            var response = await client.SendEmailAsync(msg);

            Console.WriteLine($" Message: {body}");
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

