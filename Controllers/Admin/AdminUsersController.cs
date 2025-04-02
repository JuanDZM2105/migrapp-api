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
    
    public class AdminUsersController : ControllerBase
    {
        private readonly IAdminUserService _adminUserService;
        private readonly IUserRepository _userRepository;
        private readonly IColumnVisibilityService _columnVisibilityService;

        public AdminUsersController(IAdminUserService adminUserService, IUserRepository userRepository, IColumnVisibilityService columnVisibilityService) 
        {
            _adminUserService = adminUserService;
            _userRepository = userRepository;
            _columnVisibilityService = columnVisibilityService;
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

        [HttpPatch("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                // Llamar al servicio para actualizar el perfil
                var result = await _adminUserService.UpdateUserProfileAsync(userId, dto);

                if (!result)
                {
                    return BadRequest(new { message = "Error al actualizar el perfil" });
                }

                return Ok(new { message = "Perfil actualizado con éxito" });
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

        [HttpGet("columns/available")]
        public async Task<IActionResult> GetAvailableColumns()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var columns = await _columnVisibilityService.GetAvailableColumnsAsync(userId);
            return Ok(columns);
        }

        [HttpPost("columns")]
        public async Task<IActionResult> SaveColumnVisibility([FromBody] SaveColumnVisibilityDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _columnVisibilityService.SaveColumnVisibilityAsync(userId, dto);
            return Ok(new { message = "Configuración de columnas guardada" });
        }

        [HttpGet("full-info")]
        [Authorize]
        public async Task<IActionResult> GetUsersWithFullInfo([FromQuery] UserQueryParams queryParams)
        {
            try
            {
                // Obtener el userId del token JWT
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                // Llamar al servicio que manejará la lógica de obtención de datos
                var users = await _adminUserService.GetUsersWithFullInfoAsync(queryParams, userId);

                if (users == null || !users.Any())
                {
                    return NotFound(new { message = "No se encontraron usuarios. 1" });
                }

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocurrió un error al obtener los usuarios", details = ex.Message });
            }
        }

        [HttpGet("{userId}/info")]
        [Authorize]
        public async Task<IActionResult> GetUserInfo(int userId)
        {
            try
            {
                var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)); 

                var userInfo = await _adminUserService.GetUserInfoAsync(userId, currentUserId);

                if (userInfo == null)
                {
                    return NotFound(new { message = "Usuario no disponible." });
                }

                return Ok(userInfo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocurrió un error al obtener la información del usuario", details = ex.Message });
            }
        }
    }
}
