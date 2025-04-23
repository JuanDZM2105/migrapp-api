using migrapp_api.DTOs.Admin;
using System.Threading.Tasks;

namespace migrapp_api.Services.Admin
{
    public interface IColumnVisibilityService
    {
        Task<AvailableColumnsDto> GetAvailableColumnsAsync(int UserId);
        Task SaveColumnVisibilityAsync(int userId, SaveColumnVisibilityDto dto);
    }
}
