using System;
using System.ComponentModel.DataAnnotations;

namespace migrapp_api.Entidades
{
    public class Document
    {
        [Key]
        public int DocumentId { get; set; }

        [Required, MaxLength(100)]
        public string DocumentType { get; set; }

        [Required]
        public string FilePath { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        // Relaciones
        public int UserId { get; set; }
        public User User { get; set; }
    }
}

