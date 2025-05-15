using System.ComponentModel.DataAnnotations;

namespace migrapp_api.DTOs.Admin
{
    public class UploadDocumentDto
    {
        [Required]
        public IFormFile File { get; set; }  // Para capturar el archivo que el administrador va a subir

        [Required]
        public string Name { get; set; }

        [Required]
        public string Type { get; set; }  // Tipo de documento (PDF, Word, etc.)

        [Required]
        public int UserId { get; set; }
    }
}
