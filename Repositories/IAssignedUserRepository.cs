using migrapp_api.Models;

namespace migrapp_api.Repositories
{
    public interface IAssignedUserRepository
    {
        Task AddAsync(AssignedUser assignedUser);
        Task SaveChangesAsync();
    }
}
