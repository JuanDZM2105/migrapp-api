using migrapp_api.Entidades;
using System.Threading.Tasks;

namespace migrapp_api.Services.Admin
{
    public interface ILogService
    {
        Task LogActionAsync(int userId, string actionType, string description, string ipAddress);
    }
}