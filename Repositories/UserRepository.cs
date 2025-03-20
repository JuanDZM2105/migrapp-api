using Microsoft.EntityFrameworkCore;
using migrapp_api.Data;
using migrapp_api.Entidades;

namespace migrapp_api.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<User>> GetFilteredUsersAsync(
            string? userType = null,
            string? accountStatus = null,
            DateTime? lastLoginFrom = null,
            DateTime? lastLoginTo = null,
            bool? isActiveNow = null,
            string? searchQuery = null
        )
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(userType))
                query = query.Where(u => u.UserType == userType);

            if (!string.IsNullOrWhiteSpace(accountStatus))
                query = query.Where(u => u.AccountStatus == accountStatus);

            if (lastLoginFrom.HasValue)
                query = query.Where(u => u.LastLogin >= lastLoginFrom.Value);

            if (lastLoginTo.HasValue)
                query = query.Where(u => u.LastLogin <= lastLoginTo.Value);

            if (isActiveNow.HasValue)
                query = query.Where(u => u.IsActiveNow == isActiveNow.Value);

            if (!string.IsNullOrWhiteSpace(searchQuery))
                query = query.Where(u => u.Name.Contains(searchQuery)
                                      || u.Email.Contains(searchQuery)
                                      || (u.Phone != null && u.Phone.Contains(searchQuery)));

            return await query.ToListAsync();
        }

        public async Task<User?> GetAdminUserByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.UserType != "User");
        }
    }
}
