using Microsoft.AspNetCore.Identity;
using migrapp_api.DTOs.Auth;
using migrapp_api.Entidades;
using migrapp_api.Helpers;
using migrapp_api.Repositories;

public class LoginService : ILoginService
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailHelper _emailHelper;
    private readonly ISmsHelper _smsHelper;
    private readonly IMfaCodeRepository _mfaCodeRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginService(IUserRepository userRepository,
                        IEmailHelper emailHelper,
                        ISmsHelper smsHelper,
                        IMfaCodeRepository mfaCodeRepository,
                        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _emailHelper = emailHelper;
        _smsHelper = smsHelper;
        _mfaCodeRepository = mfaCodeRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
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

        var code = new Random().Next(100000, 999999).ToString();

        await _mfaCodeRepository.SaveCodeAsync(email, code);

        if (mfaMethod == "email")
            await _emailHelper.SendEmailAsync(user.Email, "Tu código de verificación", $"Tu código es: {code}");
        else if (mfaMethod == "sms")
            await _smsHelper.SendSmsAsync(user.PhonePrefix + user.Phone, $"Tu código es: {code}");

        return code;
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

        var token = _jwtTokenGenerator.GenerateToken(user.Email, user.UserType);

        return new AuthResponseDto
        {
            Token = token,
            Email = user.Email,
            UserType = user.UserType
        };
    }
}
