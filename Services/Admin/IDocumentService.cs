using migrapp_api.DTOs.Admin;
using migrapp_api.Models;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using migrapp_api.Data;
using SendGrid.Helpers.Mail;


namespace migrapp_api.Services.Admin
{
    public interface IDocumentService
    {
        Task<Document> UploadDocumentAsync(UploadDocumentDto dto);
        Task<List<DocumentDto>> GetDocumentsByUserIdExcludingProcedureDocsAsync(int userId);
        Task<Document> GetDocumentByIdAndUserIdAsync(int documentId);
    }

    public class DocumentService : IDocumentService
    {
        private readonly string _storagePath;
        private readonly long _maxFileSize;
        private readonly List<string> _allowedExtensions;
        private readonly ApplicationDbContext _context;

        public DocumentService(IConfiguration configuration, ApplicationDbContext context)
        {
            _context = context;
            _storagePath = configuration["UploadSettings:StoragePath"];
            _maxFileSize = Convert.ToInt64(configuration["UploadSettings:MaxFileSizeBytes"]);
            _allowedExtensions = configuration.GetSection("UploadSettings:AllowedExtensions")
                .Get<List<string>>();
        }

        public async Task<Document> UploadDocumentAsync(UploadDocumentDto dto)
        {
            if (dto.File.Length > _maxFileSize)
            {
                throw new InvalidOperationException("El archivo es demasiado grande.");
            }

            var extension = Path.GetExtension(dto.File.FileName).ToLower();
            if (!_allowedExtensions.Contains(extension))
            {
                throw new InvalidOperationException("La extensión del archivo no está permitida.");
            }

            var fileName = Guid.NewGuid().ToString() + extension;
            var filePath = Path.Combine(_storagePath, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                if(dto.File.Length < 1)
                {
                    filePath = "hola";
                }
            }

            var document = new Document
            {
                Name = dto.Name,
                Type = dto.Type,
                FilePath = filePath,
                UserId = dto.UserId
            };

            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            return document;
        }

        public async Task<List<DocumentDto>> GetDocumentsByUserIdExcludingProcedureDocsAsync(int userId)
        {
            // Obtener los DocumentId que están en ProcedureDocuments (filtrado por DocumentId != null)
            var procedureDocumentIds = _context.ProcedureDocuments
                .Where(pd => pd.DocumentId != null)
                .Select(pd => pd.DocumentId.Value);

            // Traer documentos del usuario que no están en la lista anterior
            var documents = await _context.Documents
                .Where(d => d.UserId == userId && !procedureDocumentIds.Contains(d.Id))
                .Select(d => new DocumentDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    Type = d.Type,
                    FilePath = d.FilePath,
                    UploadedAt = d.UploadedAt
                })
                .ToListAsync();

            return documents;
        }

        public async Task<Document> GetDocumentByIdAndUserIdAsync(int documentId)
        {
            return await _context.Documents
                .FirstOrDefaultAsync(d => d.Id == documentId);
        }
    }


}
