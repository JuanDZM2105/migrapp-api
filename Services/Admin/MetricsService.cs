using Microsoft.Identity.Client;
using migrapp_api.DTOs.Admin;
using migrapp_api.Repositories;

namespace migrapp_api.Services.Admin
{
    public class MetricsService : IMetricsService
    {
        private readonly IMetricsRepository _metricsRepository;
        private readonly IUserRepository _userRepository;

        public MetricsService(IMetricsRepository metricsRepository, IUserRepository userRepository)
        {
            _metricsRepository = metricsRepository;
            _userRepository = userRepository;
        }

        public async Task<MetricsDto> GetMetricsByUserType(int userId)
        {
            var currentUser = await _userRepository.GetByIdAsync(userId);

            if (currentUser.Type == "admin")
            {
                return await GetAdminMetrics();
            }
            else if (currentUser.Type == "lawyer")
            {
                return await GetLawyerMetrics(userId);
            }
            else
            {
                throw new Exception("Tipo de usuario no válido");
            }
        }

        public async Task<MetricsDto> GetAdminMetrics()
        {
            var totalAdmins = await _metricsRepository.GetTotalAdminsAsync();
            var totalLawyers = await _metricsRepository.GetTotalLawyersAsync();
            var totalAuditors = await _metricsRepository.GetTotalAuditorsAsync();
            var totalReaders = await _metricsRepository.GetTotalReadersAsync();
            var totalUsersRole = await _metricsRepository.GetTotalUsersRoleAsync();
            var totalActiveUsers = await _metricsRepository.GetTotalActiveUsers();
            var totalEliminatedAccountStatus = await _metricsRepository.GetTotalEliminatedAccountStatus();
            var totalActiveAccountStatus = await _metricsRepository.GetTotalActiveAccountStatus();
            var totalBlockedAccountStatus = await _metricsRepository.GetTotalBlockedAccountStatus();
            var openLegalProcesses = await _metricsRepository.GetOpenLegalProcessesCount();
            var completedLegalProcesses = await _metricsRepository.GetCompletedLegalProcessesCount();


            var metricsDto = new MetricsDto
            {
                TotalAdmins = totalAdmins,
                TotalLawyers = totalLawyers,
                TotalAuditors = totalAuditors,
                TotalReaders = totalReaders,
                TotalUsers = totalUsersRole,
                TotalActiveUsers = totalActiveUsers,
                TotalEliminatedAccountStatus = totalEliminatedAccountStatus,
                TotalActiveAccountStatus = totalActiveAccountStatus,
                TotalBlockedAccountStatus = totalBlockedAccountStatus,
                TotalOpenLegalProcesses = openLegalProcesses,
                TotalCompletedLegalProcesses = completedLegalProcesses

            };

            return metricsDto;

        }

        public async Task<MetricsDto> GetLawyerMetrics(int lawyerId)
        {
            var assignedCasesCount = await _metricsRepository.GetAssignedCasesCount(lawyerId);
            var (newCases, inProcessCases, endedCases) = await _metricsRepository.GetCasesByStatus(lawyerId);
            var assignedUsersCount = await _metricsRepository.GetAssignedUsersCount(lawyerId);
            var (activeUsers, blockedUsers, eliminatedUsers) = await _metricsRepository.GetUsersStatusCountsAssignedToProfessional(lawyerId);

            var metricsDto = new MetricsDto
            {
                AssignedCasesCount = assignedCasesCount,
                NewCases = newCases,
                InProcessCases = inProcessCases,
                EndedCases = endedCases,
                TotalActiveUsers = activeUsers,
                TotalBlockedAccountStatus = blockedUsers,
                TotalEliminatedAccountStatus = eliminatedUsers,


            };
            return metricsDto;
        }
    }

}
