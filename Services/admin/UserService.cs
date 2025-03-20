using migrapp_api.DTOs;
using migrapp_api.Entidades;
using migrapp_api.Repositories;
using AutoMapper;

namespace migrapp_api.Services.admin
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserDTO?> GetUserAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return user is not null ? _mapper.Map<UserDTO>(user) : null;
        }

        public async Task<UserDTO?> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            return user is not null ? _mapper.Map<UserDTO>(user) : null;
        }

        public async Task<IEnumerable<UserDTO>> GetFilteredUsersAsync(
            string? userType = null,
            string? accountStatus = null,
            DateTime? lastLoginFrom = null,
            DateTime? lastLoginTo = null,
            bool? isActiveNow = null,
            string? searchQuery = null
        )
        {
            var users = await _userRepository.GetFilteredUsersAsync(
                userType, accountStatus, lastLoginFrom, lastLoginTo, isActiveNow, searchQuery
            );

            return _mapper.Map<IEnumerable<UserDTO>>(users);
        }

        public async Task<UserDTO> CreateUserAsync(UserCreationDTO userCreationDto)
        {
            var user = _mapper.Map<User>(userCreationDto);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userCreationDto.Password);

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return _mapper.Map<UserDTO>(user);
        }

        public async Task<bool> UpdateUserAsync(int userId, UserUpdateDTO userUpdateDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user is null) return false;

            _mapper.Map(userUpdateDto, user);
            _userRepository.Update(user);

            return await _userRepository.SaveChangesAsync();
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user is null) return false;

            _userRepository.Delete(user);
            return await _userRepository.SaveChangesAsync();
        }
    }
}
