using migrapp_api.DTOs.Admin;

namespace migrapp_api.Services.Admin
{
    public interface IAdminUserService
    {
        Task<bool> CreateUserAsync(CreateUserByAdminDto dto);

        Task<UserProfileDto> GetProfileAsync(int userId);

        Task<FiltersDto> GetFiltersAsync();

        Task<List<BulkEditFieldDto>> GetBulkEditFieldsAsync();
        Task<bool> BulkEditUsersAsync(BulkEditDto dto);

        Task<bool> UpdateUserProfileAsync(int UserId, UpdateProfileDto dto);

    }
}
