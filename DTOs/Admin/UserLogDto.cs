namespace migrapp_api.DTOs.Admin
{
    public class UserLogDto
    {
        public string ActionType { get; set; }
        public string IpAddress { get; set; }
        public string Description { get; set; }
        public DateTime ActionDate { get; set; }
    }
}
