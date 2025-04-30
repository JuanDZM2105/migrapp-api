using migrapp_api.DTOs.Admin;
using migrapp_api.Models;

namespace migrapp_api.Repositories
{
    public interface IUserLogRepository
    {
        Task<IEnumerable<UserLog>> GetUserLogsAsync(int userId);
        Task<IEnumerable<UserLog>> GetFilteredUserLogsAsync(int userId, UserLogQueryParams queryParams);
        Task<UserLogFiltersDto> GetUserLogFiltersAsync(int userId);
        Task AddAsync(UserLog userLog);
        Task<IEnumerable<UserLog>> GetAllFilteredLogsAsync(UserLogQueryParams queryParams);
        Task<int> GetTotalLogCountAsync(UserLogQueryParams queryParams); 
        Task<UserLogFiltersDto> GetAllLogFiltersAsync();
        Task<IEnumerable<UserLog>> GetLogsForUserIdsAsync(List<int> userIds, UserLogQueryParams queryParams);
        Task<int> GetTotalLogCountForUserIdsAsync(List<int> userIds, UserLogQueryParams queryParams); 
        Task<UserLogFiltersDto> GetLogFiltersForUserIdsAsync(List<int> userIds);
    }
}
