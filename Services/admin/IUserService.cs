using migrapp_api.DTOs;

namespace migrapp_api.Services.admin
{
    public interface IUserService
    {
        Task<UserDTO?> GetUserAsync(int userId);
        Task<UserDTO?> GetUserByEmailAsync(string email);
        Task<IEnumerable<UserDTO>> GetFilteredUsersAsync(
            string? userType = null,
            string? accountStatus = null,
            DateTime? lastLoginFrom = null,
            DateTime? lastLoginTo = null,
            bool? isActiveNow = null,
            string? searchQuery = null
        );

        Task<UserDTO> CreateUserAsync(UserCreationDTO userCreationDto);
        Task<bool> UpdateUserAsync(int userId, UserUpdateDTO userUpdateDto);
        Task<bool> DeleteUserAsync(int userId);
    }
}
