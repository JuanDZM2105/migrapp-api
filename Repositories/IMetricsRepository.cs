using migrapp_api.Models;

namespace migrapp_api.Repositories
{
    public interface IMetricsRepository
    {
        Task<int> GetTotalAdminsAsync();
        Task<int> GetTotalLawyersAsync();
        Task<int> GetTotalAuditorsAsync();
        Task<int> GetTotalReadersAsync();
        Task<int> GetTotalUsersRoleAsync();
        Task<int> GetTotalActiveUsers();
        Task<int> GetTotalEliminatedAccountStatus();
        Task<int> GetTotalActiveAccountStatus();
        Task<int> GetTotalBlockedAccountStatus();
        Task<int> GetOpenLegalProcessesCount();
        Task<int> GetCompletedLegalProcessesCount();

        Task<int> GetAssignedCasesCount(int lawyerId );
        Task<(int newCases, int inProcessCases, int endedCases)> GetCasesByStatus(int lawyerId);
        Task<int> GetAssignedUsersCount(int lawyerId);
        Task<(int activeUsers, int blockedUsers, int eliminatedUsers)> GetUsersStatusCountsAssignedToProfessional(int userId);










    }
}
