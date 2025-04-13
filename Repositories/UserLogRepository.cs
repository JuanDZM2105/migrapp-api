using Microsoft.EntityFrameworkCore;
using migrapp_api.Data;
using migrapp_api.DTOs.Admin;
using migrapp_api.Entidades;
using migrapp_api.Repositories;


namespace migrapp_api.Repositories
{
    public class UserLogRepository : IUserLogRepository
    {
        private readonly ApplicationDbContext _context;

        public UserLogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserLog>> GetUserLogsAsync(int userId)
        {
            return await _context.UserLogs
                                 .Where(log => log.UserId == userId)
                                 .ToListAsync(); // Sin paginación
        }

        public async Task AddAsync(UserLog userLog)
        {
            await _context.UserLogs.AddAsync(userLog);
            await _context.SaveChangesAsync();
        }
    }

}
