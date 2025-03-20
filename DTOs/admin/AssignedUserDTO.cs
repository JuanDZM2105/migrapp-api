namespace migrapp_api.DTOs.admin
{
    public class AssignedUserDTO
    {
        public int AssignedUserId { get; set; }
        public UserDTO ClientUser { get; set; }
        public UserDTO ProfessionalUser { get; set; }
        public string ProfessionalRole { get; set; }
        public DateTime AssignedAt { get; set; }
    }

}
