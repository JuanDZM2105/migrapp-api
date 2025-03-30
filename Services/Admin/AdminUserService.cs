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
                // Filtros estáticos, puedes modificar esto si prefieres que se obtengan de la base de datos
                UserTypes = new List<string> { "user", "admin", "lawyer", "auditor" },
                AccountStatuses = new List<string> { "active", "blocked", "eliminated" },
                Countries = await _userRepository.GetDistinctCountriesAsync(), // Método en el repositorio
                IsActiveNow = new List<bool> { true, false }
            };

            return filters;
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
    }
}
