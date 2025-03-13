using System.ComponentModel.DataAnnotations;

namespace migrapp_api.DTOs
{
    public class UsersCreateDTO
    {
        public string name { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public required string email { get; set; }
        [Required]
        public required string phone { get; set; }
        [Required]
        public required string password { get; set; }
    }
}
