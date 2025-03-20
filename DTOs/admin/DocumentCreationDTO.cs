namespace migrapp_api.DTOs.admin
{
    public class DocumentCreationDTO
    {
        public int UserId { get; set; }
        public string DocumentType { get; set; }
        public IFormFile File { get; set; }
    }

}
