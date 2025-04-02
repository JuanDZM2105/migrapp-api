namespace migrapp_api.DTOs.Admin
{
    public class UserInfoDto
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string UserType { get; set; }
        public string AccountStatus { get; set; }
        public bool IsActiveNow { get; set; }
    }
}
