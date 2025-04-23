using migrapp_api.DTOs.Auth;

public interface ILoginService
{
    Task<bool> ValidateUserCredentialsAsync(LoginDto dto);
    Task<string> GenerateAndSendMfaCodeAsync(string email, string mfaMethod);
    Task<bool> VerifyMfaCodeAsync(string email, string code);
    Task<AuthResponseDto?> VerifyCodeAndGenerateTokenAsync(string email, string code, bool rememberMe);
}
