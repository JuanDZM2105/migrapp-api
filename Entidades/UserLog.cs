using System;
using System.ComponentModel.DataAnnotations;

namespace migrapp_api.Entidades
{
    public class UserLog
    {

        [Key]
        public int UserLogId { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        [Required, MaxLength(50)]
        public string ActionType { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }

        [MaxLength(100)]
        public string IpAddress { get; set; }

        public DateTime ActionDate { get; set; } = DateTime.UtcNow;
    }
}
