namespace migrapp_api.DTOs.Auth
{
    public class RegisterDto
    {
        public string Email { get; set; } = null!;
        public string PhonePrefix { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
    }
}
