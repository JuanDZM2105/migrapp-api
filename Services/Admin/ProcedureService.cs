using migrapp_api.Data;
using migrapp_api.DTOs.Admin;
using migrapp_api.Models;

namespace migrapp_api.Services.Admin
{
    public class ProcedureService : IProcedureService
    {
        private readonly ApplicationDbContext _context;

        public ProcedureService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Procedure> CreateProcedureAsync(CreateProcedureDto dto)
        {
            // Verificamos que el proceso legal exista
            var legalProcess = await _context.LegalProcesses.FindAsync(dto.LegalProcessId);
            if (legalProcess == null)
            {
                throw new InvalidOperationException("El proceso legal asociado no existe.");
            }

            // Creamos el nuevo procedimiento
            var procedure = new Procedure
            {
                Name = dto.Name,
                Status = dto.Status,
                DueDate = dto.DueDate,
                LegalProcessId = dto.LegalProcessId,
            };

            _context.Procedures.Add(procedure);
            await _context.SaveChangesAsync();

            return procedure;

        }
    }
}
