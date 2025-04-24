using Microsoft.AspNetCore.Mvc;
using migrapp_api.DTOs.Auth;
using migrapp_api.Models;

public interface ILoginService
{
    Task<bool> ValidateUserCredentialsAsync(LoginDto dto);
    Task<string> GenerateAndSendMfaCodeAsync(string email, string mfaMethod);
    Task<bool> VerifyMfaCodeAsync(string email, string code);
    Task<AuthResponseDto?> VerifyCodeAndGenerateTokenAsync(string email, string code, bool rememberMe);

    Task<AuthResponseDto?> VerifyTrustedDevice(HttpContext context, User user, bool rememberMe);

    Task<bool> CreateTrustedDevice(HttpContext context, string email, bool rememberMe);
}
