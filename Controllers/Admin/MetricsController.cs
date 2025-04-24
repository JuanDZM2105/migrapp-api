using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using migrapp_api.Services.Admin;
using System.Security.Claims;

namespace migrapp_api.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/metrics")]
    public class MetricsController : ControllerBase
    {
        private readonly IMetricsService _metricsService;

        public MetricsController(IMetricsService metricsService)
        {
            _metricsService = metricsService;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetMetrics()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var metrics = await _metricsService.GetMetricsByUserType(userId);

            return Ok(metrics);

        }
    }

}
