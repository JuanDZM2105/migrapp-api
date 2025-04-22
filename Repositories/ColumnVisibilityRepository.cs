using Microsoft.EntityFrameworkCore;
using migrapp_api.Data;
using migrapp_api.Models;
using System.Threading.Tasks;

namespace migrapp_api.Repositories
{
    public class ColumnVisibilityRepository : IColumnVisibilityRepository
    {
        private readonly ApplicationDbContext _context;

        public ColumnVisibilityRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ColumnVisibility> GetByUserIdAsync(int userId)
        {
            return await _context.ColumnVisibilities.FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task SaveColumnVisibilityAsync(ColumnVisibility columnVisibility)
        {
            _context.ColumnVisibilities.Update(columnVisibility);
            await _context.SaveChangesAsync();
        }
    }
}
