﻿using migrapp_api.Entidades;

public interface IUserRepository
{
    Task AddAsync(User user);
    Task SaveChangesAsync();
    Task<List<User>> GetUsersByTypeAsync(string userType);
    Task<User?> GetByEmailAsync(string email);

    Task<User> GetByIdAsync(int userId);
    Task<List<string>> GetDistinctCountriesAsync();
}
