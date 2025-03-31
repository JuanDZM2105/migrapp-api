namespace migrapp_api.DTOs.Admin
{
    public class UserDto
    {
        public int? UserId { get; set; }
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? PhonePrefix { get; set; }
        public string? Country { get; set; }
        public string? UserType { get; set; }
        public string? AccountStatus { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? AccountCreated { get; set; }
        public DateTime? LastLogin { get; set; }
        public bool? IsActiveNow { get; set; }
    }

}
