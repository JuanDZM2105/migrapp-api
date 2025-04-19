using System.ComponentModel.DataAnnotations;

namespace migrapp_api.Models
{
    public class ProcedureDocument
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Type { get; set; }

        public bool IsUploaded { get; set; } = false;

        public int? DocumentId { get; set; }
        public Document Document { get; set; }

        // Relationships
        public int ProcedureId { get; set; }
        public Procedure Procedure { get; set; }
    }
}
