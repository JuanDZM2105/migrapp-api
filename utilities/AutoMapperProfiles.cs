using AutoMapper;
using migrapp_api.DTOs;
using migrapp_api.Entidades;

namespace migrapp_api.utilities
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            configMapUsers();
        }

        private void configMapUsers()
        {
            CreateMap<UsersCreateDTO, User>();
            CreateMap<User, UserDTO>();
        }
    }
}
