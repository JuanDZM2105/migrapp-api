using System;
using System.ComponentModel.DataAnnotations;

namespace migrapp_api.Entidades
{
    public class Document
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        [Required, MaxLength(100)]
        public string Type { get; set; }

        [Required]
        public string FilePath { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        // Relationships
        public int UserId { get; set; }
        public User User { get; set; }
    }
}

