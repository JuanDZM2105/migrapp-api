using migrapp_api.Models;
using System.Threading.Tasks;

namespace migrapp_api.Services.Admin
{
    public interface ILogService
    {
        Task LogActionAsync(int userId, string actionType, string description, string ipAddress);
    }
}