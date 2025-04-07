using System.ComponentModel.DataAnnotations;

namespace migrapp_api.Entidades
{
    public class LegalProcessDocument
    {
        [Key]
        public int LegalProcessDocumentId { get; set; }

        [Required, MaxLength(100)]
        public string RequiredDocumentType { get; set; }

        public bool IsUploaded { get; set; } = false;

        public int? DocumentId { get; set; }
        public Document Document { get; set; }

        // Relationships
        public int LegalProcessId { get; set; }
        public LegalProcess LegalProcess { get; set; }
    }
}
