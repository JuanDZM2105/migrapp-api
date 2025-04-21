using Microsoft.AspNetCore.Mvc;
using migrapp_api.Data;
using migrapp_api.DTOs.Auth;
using migrapp_api.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System;
using OtpNet;
using migrapp_api.Services.User;
using UserModel = migrapp_api.Models.User;


namespace migrapp_api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _appDbContext;
        public AuthController(ApplicationDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (model == null)
                return BadRequest(new { message = "Datos inválidos" });

            var secretKey = KeyGeneration.GenerateRandomKey(20);
            var base32SecretKey = Base32Encoding.ToString(secretKey);

            var user = new UserModel()
            {
                Email = model.Email,
                PhonePrefix = model.PhonePrefix,
                Phone = model.Phone,
                PasswordHash = model.PasswordHash,
                OtpSecretKey = base32SecretKey
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
        public async Task<IActionResult> Login([FromBody] PassWordLogin model)
        {
            if (model == null)
                return BadRequest(new { message = "Datos inválidos" });

            var userFound = await _appDbContext.Users
                .FirstOrDefaultAsync(u => u.Email == model.Email && u.PasswordHash == model.PasswordHash);

            if (userFound == null)
                return Unauthorized(new { message = "Credenciales incorrectas" });

            if (!userFound.IsActiveNow)
                return Unauthorized(new { message = "Tu cuenta está desactivada. Solicita reactivación para continuar." });

            var claims = new List<Claim>
            {
            new Claim(ClaimTypes.Name, userFound.Name),
            new Claim(ClaimTypes.Email, userFound.Email)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var properties = new AuthenticationProperties
            { 
                AllowRefresh = true,
                IsPersistent = model.RememberMe,
                ExpiresUtc = model.RememberMe
                    ? DateTimeOffset.UtcNow.AddDays(30) 
                    : DateTimeOffset.UtcNow.AddHours(1) 
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                properties
            );

            return Ok(new { message = "Inicio de sesión exitoso", user = userFound });
        }

        public class PassWordLogin
        {
            public required string Email { get; set; }       // Email del usuario
            public required string PasswordHash { get; set; } // Hash de la contraseña

            public bool RememberMe { get; set; } = false;
        }

    }
}