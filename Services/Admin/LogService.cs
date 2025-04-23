using migrapp_api.Entidades;
using migrapp_api.Repositories;
using System;
using System.Threading.Tasks;

namespace migrapp_api.Services.Admin
{
    public class LogService : ILogService
    {
        private readonly IUserLogRepository _userLogRepository;

        public LogService(IUserLogRepository userLogRepository)
        {
            _userLogRepository = userLogRepository;
        }

        public async Task LogActionAsync(int userId, string actionType, string description, string ipAddress)
        {
            var userLog = new UserLog
            {
                UserId = userId,
                ActionType = actionType,
                Description = description,
                IpAddress = ipAddress,
                ActionDate = DateTime.UtcNow
            };

            await _userLogRepository.AddAsync(userLog);
        }
    }
}
