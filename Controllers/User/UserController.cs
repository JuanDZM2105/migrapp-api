using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace migrapp_api.Controllers.User
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly ApplicationDBContext _context;

        public UserController(ApplicationDBContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet("{userId}/profile")]
        public async Task<IActionResult> index(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound("Usuario no encontrado.");

            var response = new
            {
                fullName = $"{user.Name} {user.LastName}"
            };

            return Ok(response);
        }

        [HttpGet("{userId}/information")]
        public async Task<IActionResult> read(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound("Usuario no encontrado.");

            var response = new
            {
                fullName = $"{user.Name} {user.LastName}",
                email = user.Email,
                phone = $"{user.PhonePrefix}{user.Phone}",
                country = user.Country,
                birthDate = user.BirthDate
            };

            return Ok(response);

        }

        [HttpGet("{userId}/documents")]
        public async Task<IActionResult> documents(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Documents)
                .FirstOrDefaultAsync(u => u.UserID == userId);
            if (user == null)
                return NotFound("Usuario no encontrado.");

            var documents = user.Documents.Select(doc => new
            {
                doc.Id,
                doc.Name,
                doc.Type,
                doc.FilePath,
                doc.UploadedAt
            });

            return Ok(documents);
        }
    }
}