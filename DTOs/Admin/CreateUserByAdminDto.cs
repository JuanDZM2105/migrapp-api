using System.ComponentModel.DataAnnotations;

namespace migrapp_api.DTOs.Admin
{
    public class CreateUserByAdminDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string LastName { get; set; }

        [Required, MaxLength(200)]
        public string Country { get; set; }

        public string PhonePrefix { get; set; }

        [MaxLength(20)]
        public string Phone { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; }

        [Required, Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string UserType { get; set; }

        public bool HasAccessToAllUsers { get; set; } = false;

        public List<int> AssignedUserIds { get; set; } = new();
    }
}
