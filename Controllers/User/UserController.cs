using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using migrapp_api.Data;
using migrapp_api.DTOs;
using Microsoft.AspNetCore.Authorization;
using migrapp_api.Services.Admin;
using migrapp_api.Helpers.Admin;
using System.Security.Claims;

namespace migrapp_api.Controllers.User
{
    [ApiController]
    [Route("api/user")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly ApplicationDbContext _context;
        private readonly ILogService _logService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(ApplicationDbContext context, IWebHostEnvironment env, ILogService logService, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _env = env;
            _logService = logService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("{userId}/profile")]
        public async Task<IActionResult> Index(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound("Usuario no encontrado.");

            var response = new
            {
                image = user.ImageUrl
            };

            return Ok(response);
        }

        [HttpGet("{userId}/information")]
        public async Task<IActionResult> Read(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound("Usuario no encontrado.");

            var response = new
            {
                fullName = $"{user.Name} {user.LastName}",
                image = user.ImageUrl,
                email = user.Email,
                phone = $"{user.PhonePrefix}{user.Phone}",
                country = user.Country,
                birthDate = user.BirthDate
            };

            return Ok(response);

        }

        [HttpPut("{userId}/update")]
        public async Task<IActionResult> Update(int userId, [FromBody] UpdateUserDto dto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound("Usuario no encontrado.");

            if (!string.IsNullOrWhiteSpace(dto.Name)) user.Name = dto.Name;
            if (!string.IsNullOrWhiteSpace(dto.LastName)) user.LastName = dto.LastName;
            if (!string.IsNullOrWhiteSpace(dto.PhonePrefix)) user.PhonePrefix = dto.PhonePrefix;
            if (!string.IsNullOrWhiteSpace(dto.Phone)) user.Phone = dto.Phone;
            if (!string.IsNullOrWhiteSpace(dto.Country)) user.Country = dto.Country;
            if (dto.BirthDate.HasValue) user.BirthDate = dto.BirthDate.Value;
            if (!string.IsNullOrWhiteSpace(dto.ImageUrl)) user.ImageUrl = dto.ImageUrl;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            string ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            int currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? userId.ToString());
            await _logService.LogActionAsync(
                currentUserId,
                LogActionTypes.Update,
                "Usuario actualizado exitosamente",
                ipAddress);

            var response = new UserResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                LastName = user.LastName,
                PhonePrefix = user.PhonePrefix,
                Phone = user.Phone,
                Country = user.Country,
                BirthDate = user.BirthDate,
                ImageUrl = user.ImageUrl
            };

            return Ok(new { message = "Usuario actualizado exitosamente", user = response });
        }


        [HttpPost("disable/{userId}")]
        public async Task<IActionResult> Disable(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound("Usuario no encontrado");

            if (!user.IsActiveNow)
                return BadRequest("La cuenta ya est� desactivada");

            user.IsActiveNow = false;
            await _context.SaveChangesAsync();

            return Unauthorized(new { message = "Tu cuenta ha sido desactivada. Ser�s deslogueado." });
        }

        [HttpPost("reactivate/{userId}")]
        public async Task<IActionResult> Reactivate(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound("Usuario no encontrado.");

            if (user.IsActiveNow)
                return BadRequest("La cuenta ya est� activa.");

            user.IsActiveNow = true;
            await _context.SaveChangesAsync();

            return Ok("Cuenta reactivada exitosamente.");
        }
    }
}