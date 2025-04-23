using migrapp_api.DTOs.Admin;
using migrapp_api.Entidades;

namespace migrapp_api.Repositories
{
    public interface IUserLogRepository
    {
        Task<IEnumerable<UserLog>> GetUserLogsAsync(int userId);
        Task<IEnumerable<UserLog>> GetFilteredUserLogsAsync(int userId, UserLogQueryParams queryParams);
        Task<UserLogFiltersDto> GetUserLogFiltersAsync(int userId);
        Task AddAsync(UserLog userLog);
    }
}
