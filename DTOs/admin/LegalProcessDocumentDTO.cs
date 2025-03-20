namespace migrapp_api.DTOs.admin
{
    public class LegalProcessDocumentDTO
    {
        public int LegalProcessDocumentId { get; set; }
        public string RequiredDocumentType { get; set; }
        public bool IsUploaded { get; set; }
        public DocumentDTO Document { get; set; }
    }
}
