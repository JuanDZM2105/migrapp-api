using migrapp_api.Models;
using Microsoft.EntityFrameworkCore;
using migrapp_api.Data;

namespace migrapp_api.Repositories
{
    public class AssignedUserRepository : IAssignedUserRepository
    {
        private readonly ApplicationDbContext _context;

        public AssignedUserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(AssignedUser assignedUser)
        {
            await _context.Set<AssignedUser>().AddAsync(assignedUser);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<AssignedUser>> GetAssignedUsersAsync(int userId)
        {
            return await _context.Set<AssignedUser>()
                .Where(a => a.ProfessionalUserId == userId || a.ClientUserId == userId)
                .ToListAsync();
        }
    }
}
