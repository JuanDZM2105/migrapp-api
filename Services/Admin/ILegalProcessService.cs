using migrapp_api.DTOs.Admin;
using migrapp_api.Models;

namespace migrapp_api.Services.Admin
{
    public interface ILegalProcessService
    {
        Task<LegalProcess> CreateLegalProcessAsync(CreateLegalProcessDto dto, int lawyerUserId);
    }
}
