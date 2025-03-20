namespace migrapp_api.DTOs.admin
{
    public class LegalProcessDTO
    {
        public int LegalProcessId { get; set; }
        public string ProcessType { get; set; }
        public string ProcessStatus { get; set; }
        public decimal Cost { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public UserDTO ClientUser { get; set; }
        public UserDTO LawyerUser { get; set; }
    }
}
