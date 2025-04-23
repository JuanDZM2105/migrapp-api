using Microsoft.Identity.Client;
using migrapp_api.DTOs.Admin;

namespace migrapp_api.Services.Admin
{
    public interface IMetricsService
    {
        Task<MetricsDto> GetMetricsByUserType(int userId);  
        Task<MetricsDto> GetAdminMetrics();
        Task<MetricsDto> GetLawyerMetrics(int lawyerId);
    }
}
