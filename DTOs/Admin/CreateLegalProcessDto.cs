namespace migrapp_api.DTOs.Admin
{
    public class CreateLegalProcessDto
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Status { get; set; } = "Activo";
        public decimal? Cost { get; set; }
        public string PaymentStatus { get; set; } = "Pendiente";
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime? EndDate { get; set; }
        public int ClientUserId { get; set; }
        public int? LawyerUserId { get; set; }
    }
}

