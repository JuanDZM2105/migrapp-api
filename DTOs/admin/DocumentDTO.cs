﻿namespace migrapp_api.DTOs.admin
{
    public class DocumentDTO
    {
        public int DocumentId { get; set; }
        public string DocumentType { get; set; }
        public string FilePath { get; set; }
        public DateTime UploadedAt { get; set; }
    }

}
