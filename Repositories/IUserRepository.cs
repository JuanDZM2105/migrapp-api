using migrapp_api.Entidades;

public interface IUserRepository
{
    Task AddAsync(User user);
    Task SaveChangesAsync();
    Task<List<User>> GetUsersByTypeAsync(string userType);
}
