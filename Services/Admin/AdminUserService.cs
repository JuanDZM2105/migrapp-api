using migrapp_api.DTOs.Admin;
using migrapp_api.Entidades;
using migrapp_api.Repositories;
using Microsoft.AspNetCore.Identity;
using migrapp_api.Helpers.Admin;


namespace migrapp_api.Services.Admin
{
    public class AdminUserService : IAdminUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAssignedUserRepository _assignedUserRepository;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AdminUserService(
            IUserRepository userRepository,
            IAssignedUserRepository assignedUserRepository,
            IPasswordHasher<User> passwordHasher)
        {
            _userRepository = userRepository;
            _assignedUserRepository = assignedUserRepository;
            _passwordHasher = passwordHasher;
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
                AccountCreated = DateTime.UtcNow
            };

            await _userRepository.AddAsync(newUser);
            await _userRepository.SaveChangesAsync();

            // 🔸 Validación con helper: si se requiere asignar usuarios y no se asignó ninguno, lanzar excepción
            if (UserAssignmentHelper.RequiresAssignments(dto.UserType, dto.HasAccessToAllUsers)
                && (dto.AssignedUserIds == null || !dto.AssignedUserIds.Any()))
            {
                throw new Exception("Debe asignar usuarios si el nuevo usuario no tiene acceso total.");
            }

            // 🔸 Agregamos asignaciones si corresponde
            if (UserAssignmentHelper.RequiresAssignments(dto.UserType, dto.HasAccessToAllUsers))
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
    }
}
