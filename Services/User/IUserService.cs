using migrapp_api.DTOs;
using System.Collections.Concurrent;
using UserModel = migrapp_api.Models.User;

namespace migrapp_api.Services.User
{
    public interface IUserService
    {
        Task<UserModel?> GetByIdAsync(int id);
        Task<List<UserModel>> GetAllAsync();
        Task<List<OnlineUserDto>> GetOnlineUsersAsync(int currentUserId, ConcurrentDictionary<int, OnlineUserDto> onlineUsers);
    }

}
