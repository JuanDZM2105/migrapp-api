namespace migrapp_api.DTOs.Admin
{
    public class UpdateProfileDto
    {
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string? Country { get; set; }
        public string? Phone { get; set; }
        public string? PhonePrefix { get; set; }
    }
}
