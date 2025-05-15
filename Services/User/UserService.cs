using Microsoft.EntityFrameworkCore;
using migrapp_api.Data;
using migrapp_api.DTOs;
using System.Collections.Concurrent;
using UserModel = migrapp_api.Models.User;

namespace migrapp_api.Services.User
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserModel?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<List<UserModel>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<List<OnlineUserDto>> GetOnlineUsersAsync(int currentUserId, ConcurrentDictionary<int, OnlineUserDto> onlineUsers)
        {
            var onlineUserIds = onlineUsers.Keys.ToList();

            return await _context.Users
                .Select(u => new OnlineUserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    LastName = u.LastName,
                    ImageUrl = u.ImageUrl,
                    IsOnline = onlineUserIds.Contains(u.Id),
                    UnreadCount = _context.Messages.Count(x => x.ReceiverId == currentUserId && x.SenderId == u.Id && !x.IsRead)
                })
                .OrderByDescending(u => u.IsOnline)
                .ToListAsync();
        }
    }
}
