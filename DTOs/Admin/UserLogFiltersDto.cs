namespace migrapp_api.DTOs.Admin
{
    public class UserLogFiltersDto
    {
        public List<string> ActionTypes { get; set; }
        public List<string> IpAddresses { get; set; }
    }
}
