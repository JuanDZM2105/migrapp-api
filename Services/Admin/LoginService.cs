using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using migrapp_api.DTOs.Auth;
using migrapp_api.Entidades;
using migrapp_api.Helpers;
using migrapp_api.Helpers.Admin;
using migrapp_api.Repositories;
using migrapp_api.Services.Admin;
using System.Security.Cryptography;

public class LoginService : ILoginService
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailHelper _emailHelper;
    private readonly ISmsHelper _smsHelper;
    private readonly IMfaCodeRepository _mfaCodeRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly ILogService _logService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LoginService(IUserRepository userRepository,
                        IEmailHelper emailHelper,
                        ISmsHelper smsHelper,
                        IMfaCodeRepository mfaCodeRepository,
                        IJwtTokenGenerator jwtTokenGenerator,
                        ILogService logService,
                        IHttpContextAccessor httpContextAccessor)
    {
        _userRepository = userRepository;
        _emailHelper = emailHelper;
        _smsHelper = smsHelper;
        _mfaCodeRepository = mfaCodeRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _logService = logService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<bool> ValidateUserCredentialsAsync(LoginDto dto)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email);

        if (user == null || user.UserType == "user")
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

    public async Task<AuthResponseDto?> VerifyCodeAndGenerateTokenAsync(string email, string code)
    {
        var isValid = await _mfaCodeRepository.VerifyCodeAsync(email, code);
        if (!isValid) return null;

        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null) return null;

        var token = _jwtTokenGenerator.GenerateToken(user.Email, user.UserType, user.UserId);

        string ipAddress = "123.123.123.123";

        if (_logService == null)
        {
            throw new Exception("LogService no ha sido inyectado correctamente.");
        }

        if (user == null)
        {
            throw new Exception("El usuario no está disponible.");
        }

        if (string.IsNullOrEmpty(ipAddress))
        {
            throw new Exception("La dirección IP es nula o vacía.");
        }


        await _logService.LogActionAsync(
            user.UserId,
            LogActionTypes.Login,
            "Inicio de sesión exitoso",
            ipAddress);

        return new AuthResponseDto
        {
            Token = token,
            Email = user.Email,
            UserType = user.UserType
        };
    }
}
