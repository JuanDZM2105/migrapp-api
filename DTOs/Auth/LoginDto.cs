namespace migrapp_api.DTOs.Auth
{
    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string PreferredMfaMethod { get; set; } // "email" o "sms"
    }

    public class VerifyMfaDto
    {
        public string Email { get; set; }
        public string Code { get; set; }

        public bool RememberMe { get; set; } = false;
    }

    public class AuthResponseDto
    {
        public string Token { get; set; }
        public string Email { get; set; }
        public string UserType { get; set; }
    }
}
