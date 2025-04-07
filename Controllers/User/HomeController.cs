using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace migrapp_api.Controllers.User
{
    [ApiController]
    [Route("api/user/home")]
    public class HomeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public HomeController(TuDbContext context)
        {
            _context = context;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetHomeInfo(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound("Usuario no encontrado.");

            var processes = await _context.Processes
                .Where(p => p.UserID == userId)
                .Include(p => p.Procedures)
                .ToListAsync();

            var processSummaries = processes.Select(p => new
            {
                p.ProcessName,
                p.Status,
                StartDate = p.StartDate.ToString("yyyy-MM-dd"),
                EndDate = p.EndDate?.ToString("yyyy-MM-dd"),
                TotalProcedures = p.Procedures.Count,
                CompletedProcedures = p.Procedures.Count(proc => proc.Status.ToLower() == "completado"),
                Progress = p.Procedures.Any()
                    ? (int)(p.Procedures.Count(proc => proc.Status.ToLower() == "completado") * 100.0 / p.Procedures.Count)
                    : 0
            });

            var alerts = processes
                .SelectMany(p => p.Procedures)
                .Where(proc => proc.DueDate < DateTime.Now && proc.Status.ToLower() != "completado")
                .Select(proc => $"El procedimiento '{proc.ProcedureName}' del proceso '{processes.First(p => p.ProcessID == proc.ProcessID).ProcessName}' está vencido.")
                .ToList();

            var homeData = new
            {
                User = new
                {
                    user.FirstName,
                    user.LastName,
                    user.Email
                },
                Processes = processSummaries,
                Alerts = alerts,
                Notifications = new List<string>
                {
                    "Bienvenido nuevamente.",
                    "Revisa tus procesos y procedimientos asignados."
                }
            };

            return Ok(homeData);
        }
    }
}
