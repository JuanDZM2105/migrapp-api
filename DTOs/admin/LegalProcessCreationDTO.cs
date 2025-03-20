namespace migrapp_api.DTOs.admin
{
    public class LegalProcessCreationDTO
    {
        public int ClientUserId { get; set; }
        public int? LawyerUserId { get; set; }
        public string ProcessType { get; set; }
        public decimal Cost { get; set; }
        public string PaymentStatus { get; set; }
        public string ProcessStatus { get; set; }
    }
}
