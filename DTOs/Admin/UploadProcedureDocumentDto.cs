using System.ComponentModel.DataAnnotations;

namespace migrapp_api.DTOs.Admin
{
    public class UploadProcedureDocumentDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public int userId { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public int ProcedureId { get; set; }

        [Required]
        public IFormFile File { get; set; }
    }
}
