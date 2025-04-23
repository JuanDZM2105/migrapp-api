using migrapp_api.Models;
using System.Threading.Tasks;

namespace migrapp_api.Repositories
{
    public interface IColumnVisibilityRepository
    {
        Task<ColumnVisibility> GetByUserIdAsync(int userId);
        Task SaveColumnVisibilityAsync(ColumnVisibility columnVisibility);
    }
}
