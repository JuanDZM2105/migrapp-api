using Microsoft.AspNetCore.Mvc;
using migrapp_api.DTOs;
using migrapp_api.Services.admin;

namespace migrapp_api.Controllers.admin
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/users/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            var user = await _userService.GetUserAsync(id);
            return user is not null ? Ok(user) : NotFound();
        }

        // GET: api/users/email/{email}
        [HttpGet("email/{email}")]
        public async Task<ActionResult<UserDTO>> GetUserByEmail(string email)
        {
            var user = await _userService.GetUserByEmailAsync(email);
            return user is not null ? Ok(user) : NotFound();
        }

        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetFilteredUsers(
            [FromQuery] string? userType,
            [FromQuery] string? accountStatus,
            [FromQuery] DateTime? lastLoginFrom,
            [FromQuery] DateTime? lastLoginTo,
            [FromQuery] bool? isActiveNow,
            [FromQuery] string? searchQuery
        )
        {
            var users = await _userService.GetFilteredUsersAsync(
                userType, accountStatus, lastLoginFrom, lastLoginTo, isActiveNow, searchQuery
            );

            return Ok(users);
        }

        // POST: api/users
        [HttpPost]
        public async Task<ActionResult<UserDTO>> CreateUser([FromBody] UserCreationDTO userCreationDto)
        {
            var createdUser = await _userService.CreateUserAsync(userCreationDto);
            return CreatedAtAction(nameof(GetUser), new { id = createdUser.UserId }, createdUser);
        }

        // PUT: api/users/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateDTO userUpdateDto)
        {
            var result = await _userService.UpdateUserAsync(id, userUpdateDto);
            return result ? NoContent() : NotFound();
        }

        // DELETE: api/users/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userService.DeleteUserAsync(id);
            return result ? NoContent() : NotFound();
        }
    }
}
