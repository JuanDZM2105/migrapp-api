using AutoMapper;
using migrapp_api.DTOs.admin;
using migrapp_api.Entidades;


namespace migrapp_api.Profiles
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            configMapUsers();
        }

        private void configMapUsers()
        {
            CreateMap<UserCreationDTO, User>();
            CreateMap<UserUpdateDTO, User>();
            CreateMap<User, UserDTO>();

            CreateMap<DocumentCreationDTO, Document>();
            CreateMap<Document, DocumentDTO>();

            CreateMap<LegalProcessCreationDTO, LegalProcess>();
            CreateMap<LegalProcessUpdateDTO, LegalProcess>();
            CreateMap<LegalProcess, LegalProcessDTO>();

            CreateMap<LegalProcessDocumentCreationDTO, LegalProcessDocument>();
            CreateMap<LegalProcessDocument, LegalProcessDocumentDTO>();

            CreateMap<AssignedUserCreationDTO, AssignedUser>();
            CreateMap<AssignedUser, AssignedUserDTO>();

            CreateMap<UserLogCreationDTO, UserLog>();
            CreateMap<UserLog, UserLogDTO>();
        }
    }
}
