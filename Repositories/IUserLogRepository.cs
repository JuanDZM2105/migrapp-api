﻿using migrapp_api.DTOs.Admin;
using migrapp_api.Entidades;

namespace migrapp_api.Repositories
{
    public interface IUserLogRepository
    {
        Task<IEnumerable<UserLog>> GetUserLogsAsync(int userId); 
        Task AddAsync(UserLog userLog);
    }
}
