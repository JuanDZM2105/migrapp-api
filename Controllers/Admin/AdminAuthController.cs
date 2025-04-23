using Microsoft.AspNetCore.Mvc;
using migrapp_api.DTOs.Auth;
using migrapp_api.Services;
using System.Threading.Tasks;

namespace migrapp_api.Controllers
{
    [ApiController]
    [Route("api/admin/auth")]
    public class AdminAuthController : ControllerBase
    {
        private readonly ILoginService _loginService;

        public AdminAuthController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var valid = await _loginService.ValidateUserCredentialsAsync(dto);
            if (!valid) return Unauthorized(new { message = "Credenciales inválidas" });

            await _loginService.GenerateAndSendMfaCodeAsync(dto.Email, dto.PreferredMfaMethod);
            return Ok(new { message = "Código de verificación enviado" });
        }

        [HttpPost("verify-mfa")]
        public async Task<IActionResult> VerifyCode([FromBody] VerifyMfaDto dto)
        {
            var result = await _loginService.VerifyCodeAndGenerateTokenAsync(dto.Email, dto.Code, dto.RememberMe);
            if (result == null) return Unauthorized(new { message = "Código incorrecto o expirado" });

            return Ok(result);
        }
    }
}
