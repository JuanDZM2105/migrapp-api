namespace migrapp_api.DTOs.admin
{
    public class LegalProcessUpdateDTO
    {
        public int? LawyerUserId { get; set; }
        public string ProcessStatus { get; set; }
        public string PaymentStatus { get; set; }
        public decimal Cost { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
