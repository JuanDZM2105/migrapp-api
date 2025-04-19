using AutoMapper;
using migrapp_api.DTOs.Admin;
using migrapp_api.Models;


namespace migrapp_api.Profiles
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            configMapUsers();
        }

        private void configMapUsers()
        {
            CreateMap<CreateUserByAdminDto, User>();
        }
    }
}
