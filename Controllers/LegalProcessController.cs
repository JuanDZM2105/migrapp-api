using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using migrapp_api.Data;
using Microsoft.AspNetCore.Authorization;

namespace migrapp_api.Controllers
{
    [ApiController]
    [Route("api/legalProcess")]
    [Authorize]
    public class LegalProcessController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LegalProcessController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> Index(
            int userId,
            [FromQuery] string? status,
            [FromQuery] string? type,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var query = _context.LegalProcesses
                .Include(lp => lp.Procedures)
                .Where(lp => lp.ClientUserId == userId)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(lp => lp.Status.ToLower() == status.ToLower());

            if (!string.IsNullOrWhiteSpace(type))
                query = query.Where(lp => lp.Type.ToLower() == type.ToLower());

            if (startDate.HasValue)
                query = query.Where(lp => lp.StartDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(lp => lp.StartDate <= endDate.Value);

            var legalProcesses = await query.ToListAsync();

            var result = legalProcesses.Select(lp => new
            {
                LegalProcessId = lp.Id,
                lp.Type,
                lp.Status,
                Progress = lp.Procedures.Any()
                    ? (int)(lp.Procedures.Count(proc => proc.Status.ToLower() == "completado") * 100.0 / lp.Procedures.Count)
                    : 0,
                TotalProcedures = lp.Procedures.Count,
                CompletedProcedures = lp.Procedures.Count(proc => proc.Status.ToLower() == "completado")
            });

            return Ok(result);
        }


        [HttpGet("{processId}")]
        public async Task<IActionResult> Show(int processId)
        {
            var process = await _context.LegalProcesses
                .Include(lp => lp.Procedures)
                    .ThenInclude(proc => proc.ProcedureDocuments)
                        .ThenInclude(doc => doc.Document)
                .FirstOrDefaultAsync(lp => lp.Id == processId);

            if (process == null)
                return NotFound("Proceso legal no encontrado.");

            var result = new
            {
                LegalProcessId = process.Id,
                process.Type,
                process.Status,
                process.PaymentStatus,
                process.Cost,
                StartDate = process.StartDate.ToString("yyyy-MM-dd"),
                EndDate = process.EndDate?.ToString("yyyy-MM-dd"),
                Procedures = process.Procedures.Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Status,
                    DueDate = p.DueDate?.ToString("yyyy-MM-dd"),
                    Documents = p.ProcedureDocuments.Select(d => new
                    {
                        d.Id,
                        d.Name,
                        d.Description,
                        d.Type,
                        d.IsUploaded,
                        d.DocumentId,
                        FilePath = d.Document?.FilePath
                    }).ToList()
                })
            };

            return Ok(result);
        }
    }
}
