using migrapp_api.DTOs.Admin;
using migrapp_api.Models;
using migrapp_api.Repositories;
using Microsoft.AspNetCore.Identity;
using migrapp_api.Helpers.Admin;
using UserModel = migrapp_api.Models.User;
using OfficeOpenXml;
using System.IO;
using Microsoft.EntityFrameworkCore;


namespace migrapp_api.Services.Admin
{
    public class AdminUserService : IAdminUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAssignedUserRepository _assignedUserRepository;
        private readonly IPasswordHasher<UserModel> _passwordHasher;
        private readonly IColumnVisibilityRepository _columnVisibilityRepository;
        private readonly IUserLogRepository _userLogRepository;
        private readonly ILogService _logService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AdminUserService(
            IUserRepository userRepository,
            IAssignedUserRepository assignedUserRepository,
            IPasswordHasher<UserModel> passwordHasher,
            IColumnVisibilityRepository columnVisibilityRepository,
            IUserLogRepository userLogRepository,
            ILogService logService,
            IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _assignedUserRepository = assignedUserRepository;
            _passwordHasher = passwordHasher;
            _columnVisibilityRepository = columnVisibilityRepository;
            _userLogRepository = userLogRepository;
            _logService = logService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> CreateUserAsync(CreateUserByAdminDto dto)
        {
            var newUser = new UserModel
            {
                Email = dto.Email,
                Name = dto.Name,
                LastName = dto.LastName,
                Country = dto.Country,
                Phone = dto.Phone,
                PhonePrefix = dto.PhonePrefix,
                PasswordHash = _passwordHasher.HashPassword(null, dto.Password),
                AccountStatus = "active",
                Type = dto.UserType,
                AccountCreated = DateTime.UtcNow,
                HasAccessToAllUsers = dto.HasAccessToAllUsers // Aquí asignamos el valor
            };

            await _userRepository.AddAsync(newUser);
            await _userRepository.SaveChangesAsync();

            // 🔸 Validación con helper: si se requiere asignar usuarios y no se asignó ninguno, lanzar excepción
            if (UserAssignmentHelper.RequiresAssignments(dto.UserType, dto.HasAccessToAllUsers)
                && (dto.AssignedUserIds == null || !dto.AssignedUserIds.Any()) && !newUser.HasAccessToAllUsers)
            {
                throw new Exception("Debe asignar usuarios si el nuevo usuario no tiene acceso total.");
            }

            // 🔸 Si HasAccessToAllUsers es false, agregamos las asignaciones de usuarios
            if (!newUser.HasAccessToAllUsers && UserAssignmentHelper.RequiresAssignments(dto.UserType, dto.HasAccessToAllUsers))
            {
                foreach (var assignedUserId in dto.AssignedUserIds)
                {
                    var assigned = new AssignedUser
                    {
                        ClientUserId = assignedUserId,
                        ProfessionalUserId = newUser.Id,
                        ProfessionalRole = dto.UserType,
                        AssignedAt = DateTime.UtcNow
                    };
                    await _assignedUserRepository.AddAsync(assigned);
                }

                await _assignedUserRepository.SaveChangesAsync();
            }

            string ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            await _logService.LogActionAsync(
                newUser.Id,
                LogActionTypes.Create,
                $"Usuario creado: {newUser.Email}, Tipo: {newUser.Type}",
                ipAddress);

            return true;
        }


        public async Task<UserProfileDto> GetProfileAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return null;
            }

            // Mapear la entidad a un DTO
            var userProfile = new UserProfileDto
            {
                Name = user.Name,
                LastName = user.LastName,
                Email = user.Email,
                Country = user.Country,
                Phone = user.Phone,
                PhonePrefix = user.PhonePrefix,
                UserType = user.Type,
                AccountStatus = user.AccountStatus,
                AccountCreated = user.AccountCreated,
                BirthDate = user.BirthDate,
            };

            return userProfile;
        }

        public async Task<FiltersDto> GetFiltersAsync()
        {
            var filters = new FiltersDto
            {
                UserTypes = new List<string> { "user", "admin", "lawyer", "auditor" },
                AccountStatuses = new List<string> { "active", "blocked", "eliminated" },
                Countries = await _userRepository.GetDistinctCountriesAsync(), // Método en el repositorio
                IsActiveNow = new List<bool> { true, false }
            };

            return filters;
        }

        public async Task<List<BulkEditFieldDto>> GetBulkEditFieldsAsync()
        {
            // Definir los campos editables.
            var fields = new List<BulkEditFieldDto>
            {
                new BulkEditFieldDto { Field = "accountStatus", Label = "Estado de cuenta" },
                new BulkEditFieldDto { Field = "userType", Label = "Tipo de usuario" }
            };

            return fields;
        }

        public async Task<bool> BulkEditUsersAsync(BulkEditDto dto)
        {
            var validFields = new List<string> { "accountStatus", "userType" };

            if (!validFields.Contains(dto.Field))
            {
                throw new ArgumentException("Campo no válido.");
            }

            // Validamos los valores 
            if (dto.Field == "accountStatus" && !new List<string> { "active", "blocked", "eliminated" }.Contains(dto.Value))
            {
                throw new ArgumentException("Valor no válido para 'accountStatus'.");
            }

            if (dto.Field == "userType" && !new List<string> { "user", "admin", "lawyer", "auditor" }.Contains(dto.Value))
            {
                throw new ArgumentException("Valor no válido para 'userType'.");
            }

            var users = await _userRepository.GetUsersByIdsAsync(dto.UserIds);
            if (users == null || !users.Any())
            {
                throw new Exception("No se encontraron usuarios con los IDs proporcionados.");
            }

            // Modificar el campo seleccionado para los usuarios
            foreach (var user in users)
            {
                if (dto.Field == "accountStatus")
                {
                    user.AccountStatus = dto.Value;
                }
                else if (dto.Field == "userType")
                {
                    user.Type = dto.Value;
                }
            }

            await _userRepository.SaveChangesAsync();

            string ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            foreach (var userId in dto.UserIds)
            {
                await _logService.LogActionAsync(
                    userId,
                    LogActionTypes.Update,
                    $"Campo modificado en edición masiva: {dto.Field} = {dto.Value}",
                    ipAddress);
            }

            return true;
        }

        public async Task<bool> UpdateUserProfileAsync(int userId, UpdateProfileDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null) return false;

            if (!string.IsNullOrEmpty(dto.Name))
                user.Name = dto.Name;

            if (!string.IsNullOrEmpty(dto.LastName))
                user.LastName = dto.LastName;

            if (!string.IsNullOrEmpty(dto.Country))
                user.Country = dto.Country;

            if (!string.IsNullOrEmpty(dto.Phone))
                user.Phone = dto.Phone;

            if (!string.IsNullOrEmpty(dto.PhonePrefix))
                user.PhonePrefix = dto.PhonePrefix;

            // Guardar los cambios
            await _userRepository.SaveChangesAsync();
            return true;
        }

        public async Task<List<UserDto>> GetUsersWithFullInfoAsync(UserQueryParams queryParams, int userId)
        {
            // Obtener la configuración de columnas del usuario
            var columnVisibility = await _columnVisibilityRepository.GetByUserIdAsync(userId);

            // Si el usuario no tiene configuración de columnas, crear una predeterminada
            if (columnVisibility == null)
            {
                columnVisibility = new ColumnVisibility
                {
                    UserId = userId,
                    VisibleColumns = string.Join(",", new List<string> { "name", "email", "country", "phone", "accountStatus", "type" })
                };
                await _columnVisibilityRepository.SaveColumnVisibilityAsync(columnVisibility);
            }

            var visibleColumns = columnVisibility.VisibleColumns.Split(',').ToList();

            // Llamar al repositorio para obtener los usuarios
            var users = await _userRepository.GetUsersWithFullInfoAsync(queryParams, userId);

            // Convertir las entidades de usuarios a DTOs y aplicar la visibilidad de columnas
            var userDtos = users.Select(user =>
            {
                var userDto = new UserDto
                {
                    UserId = user.Id,
                    Name = visibleColumns.Contains("name") ? user.Name : null,
                    LastName = visibleColumns.Contains("lastName") ? user.LastName : null,
                    Email = visibleColumns.Contains("email") ? user.Email : null,
                    Phone = visibleColumns.Contains("phone") ? user.Phone : null,
                    PhonePrefix = visibleColumns.Contains("phonePrefix") ? user.PhonePrefix : null,
                    Country = visibleColumns.Contains("country") ? user.Country : null,
                    AccountStatus = visibleColumns.Contains("accountStatus") ? user.AccountStatus : null,
                    UserType = visibleColumns.Contains("type") ? user.Type : null,
                    BirthDate = visibleColumns.Contains("birthDate") ? user.BirthDate : null,
                    AccountCreated = visibleColumns.Contains("accountCreated") ? user.AccountCreated : null,
                    LastLogin = visibleColumns.Contains("lastLogin") ? user.LastLogin : null,
                    IsActiveNow = visibleColumns.Contains("isActiveNow") ? user.IsActiveNow : null
                };

                return userDto;
            }).ToList();

            return userDtos;
        }

        public async Task<byte[]> ExportUsersToExcelAsync(UserQueryParams queryParams, int userId)
        {
            var columnVisibility = await _columnVisibilityRepository.GetByUserIdAsync(userId);

            if (columnVisibility == null)
            {
                columnVisibility = new ColumnVisibility
                {
                    UserId = userId,
                    VisibleColumns = string.Join(",", new List<string> { "name", "email", "country", "phone", "accountStatus", "userType" })
                };
            }

            var visibleColumns = columnVisibility.VisibleColumns.Split(',').ToList();

            var users = await _userRepository.GetUsersWithFullInfoAsync(queryParams, userId);


            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Users");

                // Añadir los encabezados de columna
                int colIndex = 1;
                if (visibleColumns.Contains("name"))
                    worksheet.Cells[1, colIndex++].Value = "Name";

                if (visibleColumns.Contains("email"))
                    worksheet.Cells[1, colIndex++].Value = "Email";

                if (visibleColumns.Contains("country"))
                    worksheet.Cells[1, colIndex++].Value = "Country";

                if (visibleColumns.Contains("phone"))
                    worksheet.Cells[1, colIndex++].Value = "Phone";

                if (visibleColumns.Contains("accountStatus"))
                    worksheet.Cells[1, colIndex++].Value = "Account Status";

                if (visibleColumns.Contains("userType"))
                    worksheet.Cells[1, colIndex++].Value = "User Type";

                // Rellenar los datos de usuarios
                int rowIndex = 2;  // Comienza en la fila 2 (debajo de los encabezados)
                foreach (var user in users)
                {
                    colIndex = 1;

                    if (visibleColumns.Contains("name"))
                        worksheet.Cells[rowIndex, colIndex++].Value = user.Name;
                    if (visibleColumns.Contains("email"))
                        worksheet.Cells[rowIndex, colIndex++].Value = user.Email;
                    if (visibleColumns.Contains("country"))
                        worksheet.Cells[rowIndex, colIndex++].Value = user.Country;
                    if (visibleColumns.Contains("phone"))
                        worksheet.Cells[rowIndex, colIndex++].Value = user.Phone;
                    if (visibleColumns.Contains("accountStatus"))
                        worksheet.Cells[rowIndex, colIndex++].Value = user.AccountStatus;
                    if (visibleColumns.Contains("userType"))
                        worksheet.Cells[rowIndex, colIndex++].Value = user.Type;

                    rowIndex++;
                }

                // Retornar el archivo Excel como un array de bytes
                return package.GetAsByteArray();
            }

        }

        public async Task<UserInfoDto> GetUserInfoAsync(int userId, int currentUserId)
        {
            var currentUser = await _userRepository.GetByIdAsync(currentUserId);
            var userToFetch = await _userRepository.GetByIdAsync(userId);

            if (userToFetch == null) return null;

            if (currentUser.Type == "admin")
            {
                return MapToUserInfoDto(userToFetch);
            }

            if (userToFetch.AccountStatus == "eliminated" || userToFetch.AccountStatus == "blocked")
            {
                return null;
            }

            if (currentUser.HasAccessToAllUsers)
            {
                return MapToUserInfoDto(userToFetch);
            }

            // Verificar si el usuario está asignado a este usuario
            var assignedUsers = await _assignedUserRepository.GetAssignedUsersAsync(currentUserId);

            if (assignedUsers.Any(a => a.ClientUserId == userId || a.ProfessionalUserId == userId))
            {
                return MapToUserInfoDto(userToFetch);
            }

            return null;
        }

        private UserInfoDto MapToUserInfoDto(UserModel user)
        {
            return new UserInfoDto
            {
                Name = user.Name,
                LastName = user.LastName,
                Email = user.Email,
                Country = user.Country,
                Phone = user.Phone,
                UserType = user.Type,
                AccountStatus = user.AccountStatus,
                IsActiveNow = user.IsActiveNow,
            };
        }

        public async Task<bool> EditUserInfoAsync(int userId, EditUserInfoDto dto)
        {
            var userToEdit = await _userRepository.GetByIdAsync(userId);

            if (userToEdit == null)
            {
                return false;
            }

            string actionDescription = $"Campo {dto.Field} actualizado: ";

            switch (dto.Field.ToLower())
            {
                case "name":
                    actionDescription += $"nombre: {userToEdit.Name} -> {dto.Value}";
                    userToEdit.Name = dto.Value;
                    break;

                case "lastname":
                    actionDescription += $"apellido: {userToEdit.LastName} -> {dto.Value}";
                    userToEdit.LastName = dto.Value;
                    break;

                case "email":
                    actionDescription += $"correo: {userToEdit.Email} -> {dto.Value}";
                    userToEdit.Email = dto.Value;
                    break;

                case "country":
                    actionDescription += $"país: {userToEdit.Country} -> {dto.Value}";
                    userToEdit.Country = dto.Value;
                    break;

                case "phone":
                    actionDescription += $"teléfono: {userToEdit.Phone} -> {dto.Value}";
                    userToEdit.Phone = dto.Value;
                    break;

                case "phonenumber":
                    actionDescription += $"prefijo de teléfono: {userToEdit.PhonePrefix} -> {dto.Value}";
                    userToEdit.PhonePrefix = dto.Value;
                    break;

                case "usertype":
                    actionDescription += $"tipo de usuario: {userToEdit.Type} -> {dto.Value}";
                    userToEdit.Type = dto.Value;
                    break;

                case "accountstatus":
                    actionDescription += $"estado de cuenta: {userToEdit.AccountStatus} -> {dto.Value}";
                    userToEdit.AccountStatus = dto.Value;
                    break;

                default:
                    return false; 
            }

            await _userRepository.SaveChangesAsync();

            string ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            await _logService.LogActionAsync(
                userToEdit.Id,
                LogActionTypes.Update, 
                actionDescription,  
                ipAddress
            );

            return true;
        }

        public async Task<bool> HasAccessToUserLogs(int currentUserId, int userId)
        {
            var currentUser = await _userRepository.GetByIdAsync(currentUserId);
            var userToFetch = await _userRepository.GetByIdAsync(userId);

            if (currentUser == null || userToFetch == null)
            {
                return false;
            }

            if (currentUser.Type == "admin")
            {
                return true;
            }

            if (currentUser.HasAccessToAllUsers)
            {
                return true;
            }

            if (userToFetch.AccountStatus == "eliminated" || userToFetch.AccountStatus == "blocked")
            {
                return false;
            }

            var assignedUsers = await _assignedUserRepository.GetAssignedUsersAsync(currentUserId);

            if (assignedUsers.Any(a => a.ClientUserId == userId || a.ProfessionalUserId == userId))
            {
                return true;
            }

            return false;
        }

        public async Task<List<UserLogDto>> GetUserLogsAsync(int userId)
        {
            var logs = await _userLogRepository.GetUserLogsAsync(userId);

            return logs.Select(log => new UserLogDto
            {
                ActionType = log.ActionType,
                IpAddress = log.IpAddress,
                Description = log.Description,
                ActionDate = log.ActionDate
            }).ToList();
        }

        public async Task<List<UserLogDto>> GetFilteredUserLogsAsync(int userId, UserLogQueryParams queryParams)
        {
            var logs = await _userLogRepository.GetFilteredUserLogsAsync(userId, queryParams);

            return logs.Select(log => new UserLogDto
            {
                ActionType = log.ActionType,
                IpAddress = log.IpAddress,
                Description = log.Description,
                ActionDate = log.ActionDate
            }).ToList();
        }

        public async Task<UserLogFiltersDto> GetUserLogFiltersAsync(int userId)
        {
            return await _userLogRepository.GetUserLogFiltersAsync(userId);
        }

        public async Task<LogResponseDto> GetAllLogsAsync(UserLogQueryParams queryParams, int currentUserId)
        {
            var currentUser = await _userRepository.GetByIdAsync(currentUserId);

            if (currentUser == null)
            {
                throw new Exception("Usuario no encontrado");
            }

            if (currentUser.Type == "admin" || currentUser.Type == "reader" || currentUser.HasAccessToAllUsers)
            {
                var allLogs = await _userLogRepository.GetAllFilteredLogsAsync(queryParams);
                var totalCount = await _userLogRepository.GetTotalLogCountAsync(queryParams);

                var logDtos = allLogs.Select(log => new UserLogDto
                {
                    ActionType = log.ActionType,
                    IpAddress = log.IpAddress,
                    Description = log.Description,
                    ActionDate = log.ActionDate
                }).ToList();

                return new LogResponseDto
                {
                    Logs = logDtos,
                    TotalCount = totalCount,
                    Page = queryParams.Page,
                    PageSize = queryParams.PageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / queryParams.PageSize)
                };
            }
            else if (currentUser.Type == "lawyer" || currentUser.Type == "auditor")
            {
                var assignedUsers = await _assignedUserRepository.GetAssignedUsersAsync(currentUserId);
                var assignedUserIds = assignedUsers
                    .Where(au => au.ProfessionalUserId == currentUserId)
                    .Select(au => au.ClientUserId)
                    .ToList();

                var filteredLogs = await _userLogRepository.GetLogsForUserIdsAsync(assignedUserIds, queryParams);
                var totalCount = await _userLogRepository.GetTotalLogCountForUserIdsAsync(assignedUserIds, queryParams);

                var logDtos = filteredLogs.Select(log => new UserLogDto
                {
                    ActionType = log.ActionType,
                    IpAddress = log.IpAddress,
                    Description = log.Description,
                    ActionDate = log.ActionDate
                }).ToList();

                return new LogResponseDto
                {
                    Logs = logDtos,
                    TotalCount = totalCount,
                    Page = queryParams.Page,
                    PageSize = queryParams.PageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / queryParams.PageSize)
                };
            }

            // Para otros tipos de usuario, no tienen acceso
            return new LogResponseDto
            {
                Logs = new List<UserLogDto>(),
                TotalCount = 0,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize,
                TotalPages = 0
            };
        }

        public async Task<UserLogFiltersDto> GetAllLogFiltersAsync(int currentUserId)
        {
            var currentUser = await _userRepository.GetByIdAsync(currentUserId);

            if (currentUser == null)
            {
                throw new Exception("Usuario no encontrado");
            }

            if (currentUser.Type == "admin" || currentUser.Type == "reader")
            {
                return await _userLogRepository.GetAllLogFiltersAsync();
            }
            else if (currentUser.Type == "lawyer" || currentUser.Type == "auditor")
            {
                if (currentUser.HasAccessToAllUsers)
                {
                    return await _userLogRepository.GetAllLogFiltersAsync();
                }

                var assignedUsers = await _assignedUserRepository.GetAssignedUsersAsync(currentUserId);
                var assignedUserIds = assignedUsers
                    .Where(au => au.ProfessionalUserId == currentUserId)
                    .Select(au => au.ClientUserId)
                    .ToList();

                return await _userLogRepository.GetLogFiltersForUserIdsAsync(assignedUserIds);
            }

            // Para otros tipos de usuario, filtros vacíos
            return new UserLogFiltersDto
            {
                ActionTypes = new List<string>(),
                IpAddresses = new List<string>()
            };
        }

    }
}
