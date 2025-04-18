﻿using migrapp_api.DTOs.Admin;
using migrapp_api.Entidades;
using migrapp_api.Repositories;
using Microsoft.AspNetCore.Identity;
using migrapp_api.Helpers.Admin;
using OfficeOpenXml;
using System.IO;
using Microsoft.EntityFrameworkCore;


namespace migrapp_api.Services.Admin
{
    public class AdminUserService : IAdminUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAssignedUserRepository _assignedUserRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IColumnVisibilityRepository _columnVisibilityRepository;
        private readonly IUserLogRepository _userLogRepository;

        public AdminUserService(
            IUserRepository userRepository,
            IAssignedUserRepository assignedUserRepository,
            IPasswordHasher<User> passwordHasher,
            IColumnVisibilityRepository columnVisibilityRepository,
            IUserLogRepository userLogRepository)
        {
            _userRepository = userRepository;
            _assignedUserRepository = assignedUserRepository;
            _passwordHasher = passwordHasher;
            _columnVisibilityRepository = columnVisibilityRepository;
            _userLogRepository = userLogRepository;
        }

        public async Task<bool> CreateUserAsync(CreateUserByAdminDto dto)
        {
            var newUser = new User
            {
                Email = dto.Email,
                Name = dto.Name,
                LastName = dto.LastName,
                Country = dto.Country,
                Phone = dto.Phone,
                PhonePrefix = dto.PhonePrefix,
                PasswordHash = _passwordHasher.HashPassword(null, dto.Password),
                AccountStatus = "active",
                UserType = dto.UserType,
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
                        ProfessionalUserId = newUser.UserId,
                        ProfessionalRole = dto.UserType,
                        AssignedAt = DateTime.UtcNow
                    };
                    await _assignedUserRepository.AddAsync(assigned);
                }

                await _assignedUserRepository.SaveChangesAsync();
            }

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
                UserType = user.UserType,
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
                    user.UserType = dto.Value;  
                }
            }

            await _userRepository.SaveChangesAsync();  
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
                    VisibleColumns = string.Join(",", new List<string> { "name", "email", "country", "phone", "accountStatus", "userType" })
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
                    UserId = user.UserId,
                    Name = visibleColumns.Contains("name") ? user.Name : null,
                    LastName = visibleColumns.Contains("lastName") ? user.LastName : null,
                    Email = visibleColumns.Contains("email") ? user.Email : null,
                    Phone = visibleColumns.Contains("phone") ? user.Phone : null,
                    PhonePrefix = visibleColumns.Contains("phonePrefix") ? user.PhonePrefix : null,
                    Country = visibleColumns.Contains("country") ? user.Country : null,
                    AccountStatus = visibleColumns.Contains("accountStatus") ? user.AccountStatus : null,
                    UserType = visibleColumns.Contains("userType") ? user.UserType : null,
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
                        worksheet.Cells[rowIndex, colIndex++].Value = user.UserType;

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

            if (currentUser.UserType == "admin")
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

        private UserInfoDto MapToUserInfoDto(User user)
        {
            return new UserInfoDto
            {
                Name = user.Name,
                LastName = user.LastName,
                Email = user.Email,
                Country = user.Country,
                Phone = user.Phone,
                UserType = user.UserType,
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

            switch (dto.Field.ToLower())
            {
                case "name":
                    userToEdit.Name = dto.Value;
                    break;

                case "lastname":
                    userToEdit.LastName = dto.Value;
                    break;

                case "email":
                    userToEdit.Email = dto.Value;
                    break;

                case "country":
                    userToEdit.Country = dto.Value;
                    break;

                case "phone":
                    userToEdit.Phone = dto.Value;
                    break;

                case "phonenumber":
                    userToEdit.PhonePrefix = dto.Value;
                    break;

                case "usertype":
                    userToEdit.UserType = dto.Value;
                    break;

                case "accountstatus":
                    userToEdit.AccountStatus = dto.Value;
                    break;

                default:
                    return false; // Si el campo no es válido, no se actualiza
            }

            await _userRepository.SaveChangesAsync();
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

            if (currentUser.UserType == "admin")
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

    }
}
