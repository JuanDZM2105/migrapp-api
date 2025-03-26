﻿using migrapp_api.DTOs.Admin;

namespace migrapp_api.Services.Admin
{
    public interface IAdminUserService
    {
        Task<bool> CreateUserAsync(CreateUserByAdminDto dto);
    }
}
