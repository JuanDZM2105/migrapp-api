using System.ComponentModel.DataAnnotations;

namespace migrapp_api.Entidades
{
    public class User
    {
        public int id { get; set; }
        public string name { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public required string email { get; set; }
        [Required]
        public required string phone { get; set; }
        [Required]
        public required string password { get; set; }

    }
}
