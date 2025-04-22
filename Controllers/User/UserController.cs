using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using migrapp_api.Data;
using migrapp_api.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace migrapp_api.Controllers.User
{
    [ApiController]
    [Route("api/user")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
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

            return Ok(new { message = "Usuario actualizado exitosamente", user });
        }

        [HttpPost("disable/{userId}")]
        public async Task<IActionResult> Disable(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound("Usuario no encontrado");

            if (!user.IsActiveNow)
                return BadRequest("La cuenta ya está desactivada");

            user.IsActiveNow = false;
            await _context.SaveChangesAsync();

            return Unauthorized(new { message = "Tu cuenta ha sido desactivada. Serás deslogueado." });
        }

        [HttpPost("reactivate/{userId}")]
        public async Task<IActionResult> Reactivate(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound("Usuario no encontrado.");

            if (user.IsActiveNow)
                return BadRequest("La cuenta ya está activa.");

            user.IsActiveNow = true;
            await _context.SaveChangesAsync();

            return Ok("Cuenta reactivada exitosamente.");
        }
    }
}