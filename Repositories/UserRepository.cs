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
}

