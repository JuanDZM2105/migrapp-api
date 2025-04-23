using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace migrapp_api.Models
{
  public class LegalProcess
  {
    [Key]
    public int Id { get; set; }

    public string Name { get; set; }

    [Required, MaxLength(100)]
    public string Type { get; set; }

    [Required, MaxLength(20)]
    public string Status { get; set; }

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

    public ICollection<Procedure> Procedures { get; set; } = new List<Procedure>();
  }
}
