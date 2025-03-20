namespace migrapp_api.DTOs.admin
{
    public class UserLogCreationDTO
    {
        public int UserId { get; set; }
        public string ActionType { get; set; }
        public string Description { get; set; }
        public string IpAddress { get; set; }
    }

}
