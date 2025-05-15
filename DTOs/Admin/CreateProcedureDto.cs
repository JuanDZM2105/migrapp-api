namespace migrapp_api.DTOs.Admin
{
    public class CreateProcedureDto
    {
        public string Name { get; set; }
        public string Status { get; set; } = "Pendiente";  
        public DateTime? DueDate { get; set; }
        public int LegalProcessId { get; set; }
    }
}
