using Microsoft.AspNetCore.Identity;
using migrapp_api.DTOs.Auth;
using migrapp_api.DTOs;
using migrapp_api.Models;
using migrapp_api.Helpers;
using migrapp_api.Repositories;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using migrapp_api.Helpers.Auth;
using Microsoft.AspNetCore.Authentication.Cookies;

public class LoginService : ILoginService
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailHelper _emailHelper;
    private readonly ISmsHelper _smsHelper;
    private readonly IMfaCodeRepository _mfaCodeRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IDeviceHelper _deviceHelper;

    public LoginService(IUserRepository userRepository,
                        IEmailHelper emailHelper,
                        ISmsHelper smsHelper,
                        IMfaCodeRepository mfaCodeRepository,
                        IJwtTokenGenerator jwtTokenGenerator,
                        IDeviceHelper deviceHelper)
    {
        _userRepository = userRepository;
        _emailHelper = emailHelper;
        _smsHelper = smsHelper;
        _mfaCodeRepository = mfaCodeRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _deviceHelper = deviceHelper;
    }

    public async Task<bool> ValidateUserCredentialsAsync(LoginDto dto)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email);

        if (user == null || user.Type == "user")
            return false;

        var passwordHasher = new PasswordHasher<User>();
        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);

        return result == PasswordVerificationResult.Success;
    }

    public async Task<string> GenerateAndSendMfaCodeAsync(string email, string mfaMethod)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null) throw new Exception("Usuario no encontrado");

        // Generar código seguro con RandomNumberGenerator
        var randomNumber = new byte[6];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
        }
        var code = BitConverter.ToUInt32(randomNumber, 0) % 1000000;

        // Guardar el código en la base de datos
        await _mfaCodeRepository.SaveCodeAsync(email, code.ToString("D6"));

        // Enviar el código según el método de MFA (Email o SMS)
        if (mfaMethod == "email")
            await _emailHelper.SendEmailAsync(user.Email, "Tu código de verificación", $"Tu código es: {code:D6}");
        else if (mfaMethod == "sms")
            await _smsHelper.SendSmsAsync(user.PhonePrefix + user.Phone, $"Tu código es: {code:D6}");

        return code.ToString("D6");  // Retorna el código MFA generado
    }

    public async Task<bool> VerifyMfaCodeAsync(string email, string code)
    {
        return await _mfaCodeRepository.VerifyCodeAsync(email, code);
    }

    public async Task<AuthResponseDto?> VerifyCodeAndGenerateTokenAsync(string email, string code, bool rememberMe)
    {
        var isValid = await _mfaCodeRepository.VerifyCodeAsync(email, code);
        if (!isValid) return null;

        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null) return null;

        var token = _jwtTokenGenerator.GenerateToken(user.Email, user.Type, user.Id, rememberMe);

        return new AuthResponseDto
        {
            Token = token,
            Email = user.Email,
            UserType = user.Type,
            DeviceIsTrusted = rememberMe
        };
    }

    public async Task<AuthResponseDto?> VerifyTrustedDevice(HttpContext context, User user, bool rememberMe)
    {
        var cookie = context.Request.Cookies["trusted_device"];
        var userAgent = context.Request.Headers["User-Agent"].FirstOrDefault() ?? "unknown-device";

        if (string.IsNullOrEmpty(cookie))
        {
            // No hay cookie, no es dispositivo confiable
            return null;
        }

        var expectedToken = _deviceHelper.GenerateDeviceToken(user.Id, userAgent);

        if (cookie == expectedToken)
        {
            var token = _jwtTokenGenerator.GenerateToken(user.Email, user.Type, user.Id, rememberMe);
            return new AuthResponseDto
            {
                Token = token,
                Email = user.Email,
                UserType = user.Type,
                DeviceIsTrusted = true
            };
        }

        // Cookie no coincide, no es dispositivo confiable
        return null;
    }


    public async Task<bool> CreateTrustedDevice(HttpContext context, string email, bool rememberMe)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null) return false;

        if (rememberMe)
        {
            var userAgent = context.Request.Headers["User-Agent"].FirstOrDefault() ?? "Test-Agent"; // Por ahora para las pruebas
            if (string.IsNullOrEmpty(userAgent))
                throw new Exception("User-Agent header is missing");
            var token = _deviceHelper.GenerateDeviceToken(user.Id, userAgent);
            context.Response.Cookies.Append("trusted_device", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTimeOffset.UtcNow.AddDays(30),
                SameSite = SameSiteMode.Strict
            });

            return true;
        }

        return false;
    }

}
