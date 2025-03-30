using Microsoft.EntityFrameworkCore;
using migrapp_api.Data;
using migrapp_api.Entidades;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    public UserRepository(ApplicationDbContext context) => _context = context;

    public async Task AddAsync(User user) => await _context.Set<User>().AddAsync(user);
    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();

    public async Task<List<User>> GetUsersByTypeAsync(string userType) =>
        await _context.Set<User>().Where(u => u.UserType == userType).ToListAsync();
    public async Task<User?> GetByEmailAsync(string email) =>
    await _context.Set<User>().FirstOrDefaultAsync(u => u.Email == email);

    public async Task<User?> GetByIdAsync(int userId)
    {
        return await _context.Users
             .FirstOrDefaultAsync(u => u.UserId == userId);
    }
    public async Task<List<string>> GetDistinctCountriesAsync()
    {
        return await _context.Set<User>()
            .Where(u => !string.IsNullOrEmpty(u.Country))
            .Select(u => u.Country)
            .Distinct()
            .ToListAsync();
    }
    public async Task<List<User>> GetUsersByIdsAsync(List<int> userIds)
    {
        return await _context.Users
            .Where(u => userIds.Contains(u.UserId))  
            .ToListAsync();  
    }
}

