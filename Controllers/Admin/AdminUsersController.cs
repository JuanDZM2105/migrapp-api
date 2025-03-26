using Microsoft.AspNetCore.Mvc;
using migrapp_api.DTOs.Admin;
using migrapp_api.Services.Admin;
using migrapp_api.Repositories;

namespace migrapp_api.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/users")]
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
    }
}
