using Microsoft.AspNetCore.Mvc;
using migrapp_api.Data;
using migrapp_api.DTOs.Auth;
using migrapp_api.Models;
using Microsoft.EntityFrameworkCore;
using OtpNet;
using UserModel = migrapp_api.Models.User;


namespace migrapp_api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _appDbContext;
        private readonly ILoginService _loginService;

        public AuthController(ApplicationDbContext appDbContext, ILoginService loginService)
        {
            _appDbContext = appDbContext;
            _loginService = loginService;

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (model == null)
                return BadRequest(new { message = "Datos inválidos" });

            var secretKey = KeyGeneration.GenerateRandomKey(20);
            var base32SecretKey = Base32Encoding.ToString(secretKey);

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.PasswordHash);

            var user = new UserModel()
            {
                Name = " ",
                LastName = " ",
                Email = model.Email,
                PhonePrefix = model.PhonePrefix,
                Phone = model.Phone,
                PasswordHash = hashedPassword,
                Country = " ",
                AccountStatus = "Active",
                Type = "Client",
                LastLogin = DateTime.UtcNow,
                IsActiveNow = true,
                OtpSecretKey = base32SecretKey,
                HasAccessToAllUsers = false,

                Documents = new List<Document>(),
                ClientLegalProcesses = new List<LegalProcess>(),
                LawyerLegalProcesses = new List<LegalProcess>(),
                AssignedProfessionals = new List<AssignedUser>(),
                AssignedClients = new List<AssignedUser>(),
                UserLogs = new List<UserLog>()
            };

            await _appDbContext.Users.AddAsync(user);
            await _appDbContext.SaveChangesAsync();

            if (user.Id != 0)
            {
                return Ok(new { message = "Usuario registrado exitosamente" });
            }

            return BadRequest(new { message = "No se pudo crear el usuario" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null)
                return Unauthorized(new { message = "Usuario no encontrado" });

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized(new { message = "Credenciales inválidas" });

            var trustedResult = await _loginService.VerifyTrustedDevice(HttpContext, user, dto.RememberMe);
            if (trustedResult != null && trustedResult.DeviceIsTrusted)
            {
                return Ok(trustedResult);
            }

            await _loginService.GenerateAndSendMfaCodeAsync(user.Email, dto.PreferredMfaMethod);

            return Ok(new { message = "Código de verificación enviado" });
        }

        [HttpPost("verify-mfa")]
        public async Task<IActionResult> VerifyCode([FromBody] VerifyMfaDto dto)
        {
            var result = await _loginService.VerifyCodeAndGenerateTokenAsync(dto.Email, dto.Code, dto.RememberMe);
            if (result == null) return Unauthorized(new { message = "Código incorrecto o expirado" });

            await _loginService.CreateTrustedDevice(HttpContext, dto.Email, dto.RememberMe);

            return Ok(result);
        }

    }
}