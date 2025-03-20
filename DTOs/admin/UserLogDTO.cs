namespace migrapp_api.DTOs.admin
{
    public class UserLogDTO
    {
        public int UserLogId { get; set; }
        public string ActionType { get; set; }
        public string Description { get; set; }
        public string IpAddress { get; set; }
        public DateTime ActionDate { get; set; }
    }

}
