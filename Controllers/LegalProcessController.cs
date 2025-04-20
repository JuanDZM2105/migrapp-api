using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using migrapp_api.Data;

namespace migrapp_api.Controllers
{
    [ApiController]
    [Route("api/legalProcess")]
    public class LegalProcessController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LegalProcessController(ApplicationDbContext context)
        {
                _context = context;    
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> Index(int userId)
        {
            var legalProcesess = await _context.LegalProcesses
                .Include(lp => lp.Procedures)
                .Where(lp => lp.ClientUserId == userId)
                .ToListAsync();

            var result = legalProcesess.Select(lp => new
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
  