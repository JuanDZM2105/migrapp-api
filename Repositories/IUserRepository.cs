using migrapp_api.Entidades;

namespace migrapp_api.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetUserByEmailAsync(string email);

        Task<IEnumerable<User>> GetFilteredUsersAsync(
            string? userType = null,
            string? accountStatus = null,
            DateTime? lastLoginFrom = null,
            DateTime? lastLoginTo = null,
            bool? isActiveNow = null,
            string? searchQuery = null
        );

        Task<User?> GetAdminUserByEmailAsync(string email);
    }
}
