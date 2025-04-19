using System;
using System.ComponentModel.DataAnnotations;

namespace migrapp_api.Models
{
    public class AssignedUser
    {
        [Key]
        public int Id { get; set; }

        // Cliente asignado
        public int ClientUserId { get; set; }
        public User ClientUser { get; set; }

        // Profesional (abogado o auditor)
        public int ProfessionalUserId { get; set; }
        public User ProfessionalUser { get; set; }

        [Required, MaxLength(20)]
        public string ProfessionalRole { get; set; } // Lawyer, Auditor

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    }
}
