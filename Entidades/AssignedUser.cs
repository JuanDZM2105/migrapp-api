using System;
using System.ComponentModel.DataAnnotations;

namespace migrapp_api.Entidades
{
    public class AssignedUser
    {
        [Key]
        public int AssignedUserId { get; set; }

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
