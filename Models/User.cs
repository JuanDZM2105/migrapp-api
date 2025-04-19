using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace migrapp_api.Models
{
    public class User
    {

        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        public string LastName { get; set; }

        [Required, MaxLength(150)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public string PhonePrefix { get; set; }

        [MaxLength(20)]
        public string Phone { get; set; }

        [MaxLength(200)]
        public string Country { get; set; }

        public DateTime? BirthDate { get; set; }

        public DateTime AccountCreated { get; set; } = DateTime.UtcNow;

        [Required, MaxLength(20)]
        public string AccountStatus { get; set; }

        [Required, MaxLength(20)]
        public string Type { get; set; }

        public DateTime LastLogin { get; set; }
        public bool IsActiveNow { get; set; } = false;

        public string OtpSecretKey { get; set; } = string.Empty;


        //Relationships

        public ICollection<Document> Documents { get; set; }
        public ICollection<LegalProcess> ClientLegalProcesses { get; set; }
        public ICollection<LegalProcess> LawyerLegalProcesses { get; set; }
        public ICollection<AssignedUser> AssignedProfessionals { get; set; }
        public ICollection<AssignedUser> AssignedClients { get; set; }
        public ICollection<UserLog> UserLogs { get; set; }
    }
}

