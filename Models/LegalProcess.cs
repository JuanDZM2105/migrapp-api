using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace migrapp_api.Entidades
{
    public class LegalProcess
    {
        [Key]
        public int LegalProcessId { get; set; }

        [Required, MaxLength(100)]
        public string ProcessType { get; set; }

        [Required, MaxLength(20)]
        public string ProcessStatus { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Cost { get; set; }

        [Required, MaxLength(20)]
        public string PaymentStatus { get; set; }

        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        public DateTime? EndDate { get; set; }

        // Relationships
        public int ClientUserId { get; set; }
        public User ClientUser { get; set; }

        public int? LawyerUserId { get; set; }
        public User LawyerUser { get; set; }

        public ICollection<LegalProcessDocument> RequiredDocuments { get; set; }
    }
}
