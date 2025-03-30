using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using migrapp_api.DTOs.Admin;
using migrapp_api.Services.Admin;
using migrapp_api.Repositories;
using System.Security.Claims;

namespace migrapp_api.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/users")]
    [Authorize(Roles = "admin")] // 👈 Solo admin puede acceder
    public class AdminUsersController : ControllerBase
    {
        private readonly IAdminUserService _adminUserService;
        private readonly IUserRepository _userRepository;

        public AdminUsersController(IAdminUserService adminUserService, IUserRepository userRepository)
        {
            _adminUserService = adminUserService;
            _userRepository = userRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserByAdminDto dto)
        {
            var result = await _adminUserService.CreateUserAsync(dto);
            if (result)
                return Ok(new { message = "Usuario creado exitosamente" });

            return BadRequest(new { message = "Error al crear usuario" });
        }

        [HttpGet("available-users")]
        public async Task<IActionResult> GetAvailableUsers()
        {
            var users = await _userRepository.GetUsersByTypeAsync("user");
            return Ok(users.Select(u => new { u.UserId, u.Name, u.LastName, u.Email }));
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                // Obtener el userId del token JWT
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                // Llamar al servicio para obtener el perfil del usuario
                var userProfile = await _adminUserService.GetProfileAsync(userId);

                if (userProfile == null)
                {
                    return NotFound(new { message = "Usuario no encontrado" });
                }

                return Ok(userProfile);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Ocurrió un error interno", details = ex.Message });
            }
        }

        [HttpGet("filters")]
        public async Task<IActionResult> GetFilters()
        {
            var filters = await _adminUserService.GetFiltersAsync();
            return Ok(filters);
        }

        [HttpPatch("bulk-edit")]
        public async Task<IActionResult> BulkEdit([FromBody] BulkEditDto dto)
        {
            try
            {
                var result = await _adminUserService.BulkEditUsersAsync(dto);
                if (result)
                {
                    return Ok(new { message = "Usuarios actualizados correctamente." });
                }
                return BadRequest(new { message = "Error al actualizar usuarios." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("bulk-edit/fields")]
        public async Task<IActionResult> GetBulkEditFields()
        {
            var fields = await _adminUserService.GetBulkEditFieldsAsync();
            return Ok(fields);
        }
    }
}
