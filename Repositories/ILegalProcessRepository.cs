using Microsoft.EntityFrameworkCore;
using migrapp_api.Data;
using migrapp_api.Models;

namespace migrapp_api.Repositories
{
    public interface ILegalProcessRepository
    {
        Task<List<LegalProcess>> GetLegalProcessesByUserIdAsync(int userId);
    }

    public class LegalProcessRepository : ILegalProcessRepository
    {
        private readonly ApplicationDbContext _context;

        public LegalProcessRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<LegalProcess>> GetLegalProcessesByUserIdAsync(int userId)
        {
           
            return await _context.LegalProcesses
                .Where(lp => lp.ClientUserId == userId)
                .Include(lp => lp.Procedures) 
                .ToListAsync();
        }
    }

}
