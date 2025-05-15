using migrapp_api.Data;
using migrapp_api.DTOs.Admin;
using migrapp_api.Models;
using migrapp_api.Repositories;

namespace migrapp_api.Services.Admin
{
    public class LegalProcessService : ILegalProcessService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILegalProcessRepository _LegalProcessRepository;

        public LegalProcessService(ApplicationDbContext context, ILegalProcessRepository LegalProcessRepository
            )
        {
            _context = context;
            _LegalProcessRepository = LegalProcessRepository;
        }

        public async Task<LegalProcess> CreateLegalProcessAsync(CreateLegalProcessDto dto, int lawyerUserId)
        {
            var legalProcess = new LegalProcess
            {
                Name = dto.Name,
                Type = dto.Type,
                Status = dto.Status,
                Cost = dto.Cost ?? 0m,
                PaymentStatus = dto.PaymentStatus,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                ClientUserId = dto.ClientUserId,
                LawyerUserId = lawyerUserId
            };

            _context.LegalProcesses.Add(legalProcess);
            await _context.SaveChangesAsync();

            return legalProcess;
        }

    }
}
