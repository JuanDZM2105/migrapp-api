namespace migrapp_api.DTOs.admin
{
    public class AssignedUserCreationDTO
    {
        public int ClientUserId { get; set; }
        public int ProfessionalUserId { get; set; }
        public string ProfessionalRole { get; set; } // Lawyer or Auditor
    }

}
