using migrapp_api.Entidades;

namespace migrapp_api.Repositories
{
    public interface IAssignedUserRepository
    {
        Task AddAsync(AssignedUser assignedUser);
        Task SaveChangesAsync();
        Task<List<AssignedUser>> GetAssignedUsersAsync(int userId);
    }
}
