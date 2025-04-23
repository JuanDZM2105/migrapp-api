using Microsoft.EntityFrameworkCore;
using migrapp_api.Data;
using migrapp_api.DTOs.Admin;
using migrapp_api.Entidades;
using migrapp_api.Repositories;


namespace migrapp_api.Repositories
{
    public class UserLogRepository : IUserLogRepository
    {
        private readonly ApplicationDbContext _context;

        public UserLogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserLog>> GetUserLogsAsync(int userId)
        {
            return await _context.UserLogs
                                 .Where(log => log.UserId == userId)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<UserLog>> GetFilteredUserLogsAsync(int userId, UserLogQueryParams queryParams)
        {
            var query = _context.UserLogs.Where(log => log.UserId == userId);

            // Aplicar filtros
            if (!string.IsNullOrEmpty(queryParams.ActionType))
            {
                query = query.Where(log => log.ActionType == queryParams.ActionType);
            }

            if (!string.IsNullOrEmpty(queryParams.IpAddress))
            {
                query = query.Where(log => log.IpAddress.Contains(queryParams.IpAddress));
            }

            if (queryParams.FechaDesde.HasValue)
            {
                query = query.Where(log => log.ActionDate >= queryParams.FechaDesde.Value);
            }

            if (queryParams.FechaHasta.HasValue)
            {
                query = query.Where(log => log.ActionDate <= queryParams.FechaHasta.Value);
            }

            // Aplicar ordenamiento
            query = queryParams.SortDirection.ToLower() == "asc"
                ? ApplySorting(query, queryParams.SortBy, true)
                : ApplySorting(query, queryParams.SortBy, false);

            // Aplicar paginación
            return await query
                .Skip((queryParams.Page - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .ToListAsync();
        }

        private IQueryable<UserLog> ApplySorting(IQueryable<UserLog> query, string sortBy, bool ascending)
        {
            switch (sortBy.ToLower())
            {
                case "actiontype":
                    return ascending
                        ? query.OrderBy(log => log.ActionType)
                        : query.OrderByDescending(log => log.ActionType);
                case "ipaddress":
                    return ascending
                        ? query.OrderBy(log => log.IpAddress)
                        : query.OrderByDescending(log => log.IpAddress);
                case "description":
                    return ascending
                        ? query.OrderBy(log => log.Description)
                        : query.OrderByDescending(log => log.Description);
                case "actiondate":
                default:
                    return ascending
                        ? query.OrderBy(log => log.ActionDate)
                        : query.OrderByDescending(log => log.ActionDate);
            }
        }

        public async Task<UserLogFiltersDto> GetUserLogFiltersAsync(int userId)
        {
            var userLogs = await _context.UserLogs
                .Where(log => log.UserId == userId)
                .ToListAsync();

            return new UserLogFiltersDto
            {
                ActionTypes = userLogs.Select(log => log.ActionType).Distinct().ToList(),
                IpAddresses = userLogs.Select(log => log.IpAddress).Distinct().ToList()
            };
        }

        public async Task AddAsync(UserLog userLog)
        {
            await _context.UserLogs.AddAsync(userLog);
            await _context.SaveChangesAsync();
        }
    }

}
