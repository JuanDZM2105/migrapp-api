using Microsoft.EntityFrameworkCore;
using migrapp_api.Data;
using migrapp_api.Models;

namespace migrapp_api.Repositories
{
    public class MetricsRepository : IMetricsRepository
    {
        private readonly ApplicationDbContext _context;

        public MetricsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetTotalAdminsAsync()
        {
            return await _context.Users.CountAsync(u => u.Type == "admin");
        }

        public async Task<int> GetTotalLawyersAsync()
        {
            return await _context.Users.CountAsync(u => u.Type == "lawyer");
        }

        public async Task<int> GetTotalAuditorsAsync()
        {
            return await _context.Users.CountAsync(u => u.Type == "auditor");
        }

        public async Task<int> GetTotalReadersAsync()
        {
            return await _context.Users.CountAsync(u => u.Type == "reader");
        }

        // Método para obtener el número de usuarios de tipo "user"
        public async Task<int> GetTotalUsersRoleAsync()
        {
            return await _context.Users.CountAsync(u => u.Type == "user");
        }

        public async Task<int> GetTotalActiveUsers()
        {
            return await _context.Users.CountAsync(u => u.IsActiveNow == true);
        }
        public async Task<int> GetTotalEliminatedAccountStatus()
        {
            return await _context.Users.CountAsync(u => u.AccountStatus == "eliminated");
        }
        public async Task<int> GetTotalActiveAccountStatus()
        {
            return await _context.Users.CountAsync(u => u.AccountStatus == "active");
        }
        public async Task<int> GetTotalBlockedAccountStatus()
        {
            return await _context.Users.CountAsync(u => u.AccountStatus == "blocked");
        }
        public async Task<int> GetOpenLegalProcessesCount()
        {
            return await _context.LegalProcesses
                .Where(lp => lp.EndDate == null)
                .CountAsync();
        }

        public async Task<int> GetCompletedLegalProcessesCount()
        {
            return await _context.LegalProcesses
                .Where(lp => lp.EndDate < DateTime.Now)
                .CountAsync();
        }

        public async Task<int> GetAssignedCasesCount(int lawyerId)
        {
            return await _context.LegalProcesses
                .Where(lp => lp.LawyerUserId == lawyerId)
                .CountAsync();
        }

        public async Task<(int newCases, int inProcessCases, int endedCases)> GetCasesByStatus(int lawyerId)
        {
            var newCases = await _context.LegalProcesses
                .Where(lp => lp.LawyerUserId == lawyerId && lp.Status == "new")
                .CountAsync();

            var inProcessCases = await _context.LegalProcesses
                .Where(lp => lp.LawyerUserId == lawyerId && lp.Status == "in process")
                .CountAsync();

            var endedCases = await _context.LegalProcesses
                .Where(lp => lp.LawyerUserId == lawyerId && lp.Status == "ended")
                .CountAsync();

            return (newCases, inProcessCases, endedCases);
        }

        public async Task<int> GetAssignedUsersCount(int lawyerId)
        {
            return await _context.AssignedUsers
                .Where(au => au.ProfessionalUserId == lawyerId)
                .CountAsync();
        }

        public async Task<(int activeUsers, int blockedUsers, int eliminatedUsers)> GetUsersStatusCountsAssignedToProfessional(int userId)
        {
            var activeUsers = await _context.Users
                .Where(u => u.AssignedProfessionals.Any(a => a.ProfessionalUserId == userId)
                            && u.AccountStatus == "active"
                            && u.IsActiveNow == true)
                .CountAsync();

            var blockedUsers = await _context.Users
                .Where(u => u.AssignedProfessionals.Any(a => a.ProfessionalUserId == userId)
                            && u.AccountStatus == "blocked")
                .CountAsync();

            var eliminatedUsers = await _context.Users
                .Where(u => u.AssignedProfessionals.Any(a => a.ProfessionalUserId == userId)
                            && u.AccountStatus == "eliminated")
                .CountAsync();

            return (activeUsers, blockedUsers, eliminatedUsers);
        }
    }

}
