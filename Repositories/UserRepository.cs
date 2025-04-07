using Microsoft.EntityFrameworkCore;
using migrapp_api.Data;
using migrapp_api.DTOs.Admin;
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

    public async Task<List<User>> GetUsersWithFullInfoAsync(UserQueryParams queryParams)
    {
        var query = _context.Users.AsQueryable();

        // Filtrar por nombre si se proporciona
        if (!string.IsNullOrEmpty(queryParams.Name))
        {
            query = query.Where(u => u.Name.Contains(queryParams.Name));
        }

        // Filtrar por correo electrónico si se proporciona
        if (!string.IsNullOrEmpty(queryParams.Email))
        {
            query = query.Where(u => u.Email.Contains(queryParams.Email));
        }

        // Filtrar por tipo de usuario si se proporciona
        if (!string.IsNullOrEmpty(queryParams.UserType))
        {
            query = query.Where(u => u.UserType == queryParams.UserType);
        }

        // Filtrar por estado de cuenta si se proporciona
        if (!string.IsNullOrEmpty(queryParams.AccountStatus))
        {
            query = query.Where(u => u.AccountStatus == queryParams.AccountStatus);
        }

        // Filtrar por país si se proporciona
        if (!string.IsNullOrEmpty(queryParams.Country))
        {
            query = query.Where(u => u.Country.Contains(queryParams.Country));
        }

        // Ordenar por el campo proporcionado
        if (queryParams.SortBy == "name")
        {
            query = queryParams.SortDirection == "asc" ? query.OrderBy(u => u.Name) : query.OrderByDescending(u => u.Name);
        }
        else if (queryParams.SortBy == "email")
        {
            query = queryParams.SortDirection == "asc" ? query.OrderBy(u => u.Email) : query.OrderByDescending(u => u.Email);
        }
        // Agregar más campos si es necesario

        // Paginación
        var users = await query
            .Skip((queryParams.Page - 1) * queryParams.PageSize)
            .Take(queryParams.PageSize)
            .ToListAsync();

        return users;
    }

}

