using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Security.Claims;
using migrapp_api.DTOs;
using migrapp_api.Extenions;
using migrapp_api.Models;
using migrapp_api.Data;
using migrapp_api.Services.User;
using Microsoft.EntityFrameworkCore;
using MessageModel = migrapp_api.Models.Message;
using Microsoft.AspNetCore.Identity;

namespace migrapp_api.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IUserService _userService;
        private readonly ApplicationDbContext _context;
        public static readonly ConcurrentDictionary<int, OnlineUserDto> onlineUsers = new();

        public ChatHub(ApplicationDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var receiverIdStr = httpContext?.Request.Query["senderId"].ToString();
            int currentUserId = Context.User.GetUserId();

            var currentUser = await _userService.GetByIdAsync(currentUserId);
            var connectionId = Context.ConnectionId;

            if (currentUser == null)
            {
                Console.WriteLine($"No se encontró el usuario con ID {currentUserId}");
                return;
            }

            // Registrar o actualizar la conexión del usuario
            if (onlineUsers.ContainsKey(currentUserId))
            {
                onlineUsers[currentUserId].ConnectionId = connectionId;
            }
            else
            {
                var userDto = new OnlineUserDto
                {
                    ConnectionId = connectionId,
                    Name = currentUser.Name,
                    ImageUrl = currentUser.ImageUrl,
                    LastName = currentUser.LastName
                };

                onlineUsers.TryAdd(currentUserId, userDto);
                await Clients.AllExcept(connectionId).SendAsync("Notify", currentUser);
            }

            // Cargar mensajes si se pasa un senderId
            if (int.TryParse(receiverIdStr, out int receiverId))
            {
                await LoadMessages(receiverId);
            }
            Console.WriteLine($"[DEBUG] tipo {currentUser.Type}");
            // Si es cliente, enviarle su abogado asignado
            if (currentUser.Type != "lawyer")
            {
                Console.WriteLine($"[DEBUG] Usuario conectado: ID={currentUserId}, Tipo={currentUser.Type}");

                var assignedLawyerId = await _context.AssignedUsers
                    .Where(a => a.ClientUserId == currentUserId)
                    .Select(a => a.ProfessionalUserId)
                    .FirstOrDefaultAsync();

                Console.WriteLine($"[DEBUG] Abogado asignado para cliente {currentUserId}: {assignedLawyerId}");

                if (assignedLawyerId != 0)
                {
                    var lawyer = await _userService.GetByIdAsync(assignedLawyerId);
                    if (lawyer != null)
                    {
                        var isOnline = onlineUsers.ContainsKey(assignedLawyerId);
                        var unreadCount = await _context.Messages
                            .CountAsync(x =>
                                x.ReceiverId == currentUserId &&
                                x.SenderId == assignedLawyerId &&
                                !x.IsRead);

                        var result = new List<OnlineUserDto>
                        {
                            new OnlineUserDto
                            {
                                Id = lawyer.Id,
                                Name = lawyer.Name,
                                LastName = lawyer.LastName,
                                ImageUrl = lawyer.ImageUrl,
                                IsOnline = isOnline,
                                ConnectionId = isOnline ? onlineUsers[assignedLawyerId].ConnectionId : null,
                                UnreadCount = unreadCount
                            }
                        };

                        await Clients.Caller.SendAsync("AllLawyers", result);
                    }
                }
            }
            else if (currentUser.Type == "lawyer")
            {
                Console.WriteLine($"[DEBUG] Usuario abogado conectado: ID={currentUserId}");

                // Obtener IDs de los clientes asignados
                var assignedClientIds = await _context.AssignedUsers
                    .Where(a => a.ProfessionalUserId == currentUserId && a.ProfessionalRole == "lawyer")
                    .Select(a => a.ClientUserId)
                    .ToListAsync();

                Console.WriteLine($"[DEBUG] Clientes asignados: {string.Join(", ", assignedClientIds)}");

                if (assignedClientIds.Any())
                {
                    var clients = await _context.Users
                        .Where(u => assignedClientIds.Contains(u.Id))
                        .ToListAsync();

                    var result = new List<OnlineUserDto>();

                    foreach (var client in clients)
                    {
                        var isOnline = onlineUsers.ContainsKey(client.Id);
                        var unreadCount = await _context.Messages
                            .CountAsync(x =>
                                x.ReceiverId == currentUserId &&
                                x.SenderId == client.Id &&
                                !x.IsRead);

                        result.Add(new OnlineUserDto
                        {
                            Id = client.Id,
                            Name = client.Name,
                            LastName = client.LastName,
                            ImageUrl = client.ImageUrl,
                            IsOnline = isOnline,
                            ConnectionId = isOnline ? onlineUsers[client.Id].ConnectionId : null,
                            UnreadCount = unreadCount
                        });
                    }

                    await Clients.Caller.SendAsync("AssignedClients", result);
                }
            }
        }


 

        public async Task LoadMessages(int receiverId, int pageNumber = 1)
        {
            int pageSize = 10;
            int currentUserId = Context.User.GetUserId();

            List<MessageResponseDto> messages = await _context.Messages
                .Where(x =>
                    (x.ReceiverId == currentUserId && x.SenderId == receiverId) ||
                    (x.SenderId == currentUserId && x.ReceiverId == receiverId))
                .OrderByDescending(x => x.CreatedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .OrderBy(x => x.CreatedDate)
                .Select(x => new MessageResponseDto
                {
                    Id = x.Id,
                    Content = x.Content,
                    CreatedDate = x.CreatedDate,
                    ReceiverId = x.ReceiverId,
                    SenderId = x.SenderId
                })
                .ToListAsync();

            foreach (var message in messages.Where(m => m.ReceiverId == currentUserId && !m.IsRead))
            {
                var msg = await _context.Messages.FindAsync(message.Id);
                if (msg != null)
                {
                    msg.IsRead = true;
                }
            }

            await _context.SaveChangesAsync();

            await Clients.User(currentUserId.ToString()).SendAsync("ReceiveMessageList", messages);
        }

        public async Task SendMessage(MessageRequestDto message)
        {
            int senderId = Context.User.GetUserId();

            var newMessage = new MessageModel
            {
                SenderId = senderId,
                ReceiverId = message.ReceiverId,
                IsRead = false,
                CreatedDate = DateTime.UtcNow,
                Content = message.Content
            };

            _context.Messages.Add(newMessage);
            await _context.SaveChangesAsync();

            await Clients.User(message.ReceiverId.ToString()).SendAsync("ReceiveNewMessage", newMessage);
        }

        public async Task NotifyTyping(int receiverId)
        {
            int senderId = Context.User.GetUserId();

            if (onlineUsers.TryGetValue(receiverId, out var receiverUser))
            {
                await Clients.Client(receiverUser.ConnectionId).SendAsync("NotifyTypingToUser", senderId);
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            int userId = Context.User.GetUserId();
            onlineUsers.TryRemove(userId, out _);
            await Clients.All.SendAsync("OnlineUsers", await GetAllUsers(userId));
        }

        private async Task<IEnumerable<OnlineUserDto>> GetAllUsers(int currentUserId)
        {
            var onlineUserIds = new HashSet<int>(onlineUsers.Keys);

            var allUsers = await _userService.GetAllAsync();

            var result = allUsers.Select(u => new OnlineUserDto
            {
                Id = u.Id,
                Name = u.Name,
                LastName = u.LastName,
                ImageUrl = u.ImageUrl,
                IsOnline = onlineUserIds.Contains(u.Id),
                UnreadCount = _context.Messages.Count(x =>
                    x.ReceiverId == currentUserId &&
                    x.SenderId == u.Id &&
                    !x.IsRead)
            })
            .OrderByDescending(u => u.IsOnline)
            .ToList();

            return result;
        }
    }
}
