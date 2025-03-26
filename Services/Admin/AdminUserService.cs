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

    }
}
