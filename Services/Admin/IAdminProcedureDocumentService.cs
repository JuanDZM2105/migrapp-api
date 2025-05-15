using Microsoft.EntityFrameworkCore;
using migrapp_api.Data;
using migrapp_api.DTOs.Admin;
using migrapp_api.Models;

namespace migrapp_api.Services.Admin
{
    public interface IAdminProcedureDocumentService
    {
        Task<ProcedureDocument> UploadProcedureDocumentAsync(UploadProcedureDocumentDto dto);

        Task<ProcedureDocument> GetProcedureDocumentByIdAndUserAsync(int id);
    }

    public class AdminProcedureDocumentService : IAdminProcedureDocumentService
    {
        private readonly ApplicationDbContext _context;
        private readonly string _storagePath;
        private readonly long _maxFileSize;
        private readonly List<string> _allowedExtensions;

        public AdminProcedureDocumentService(IConfiguration configuration, ApplicationDbContext context)
        {
            _context = context;
            _storagePath = configuration["UploadSettings:StoragePath"];
            _maxFileSize = Convert.ToInt64(configuration["UploadSettings:MaxFileSizeBytes"]);
            _allowedExtensions = configuration.GetSection("UploadSettings:AllowedExtensions").Get<List<string>>();
        }

        public async Task<ProcedureDocument> UploadProcedureDocumentAsync(UploadProcedureDocumentDto dto)
        {
            if (dto.File.Length > _maxFileSize)
                throw new InvalidOperationException("El archivo es demasiado grande.");

            var extension = Path.GetExtension(dto.File.FileName).ToLower();
            if (!_allowedExtensions.Contains(extension))
                throw new InvalidOperationException("La extensión del archivo no está permitida.");

            // Guardar archivo físico
            var fileName = Guid.NewGuid().ToString() + extension;
            var filePath = Path.Combine(_storagePath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.File.CopyToAsync(stream);
            }

            // Crear Document asociado
            var document = new Document
            {
                Name = dto.Name,
                Type = dto.Type,
                FilePath = filePath,
                UserId = dto.userId,
                UploadedAt = DateTime.UtcNow
            };
            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            // Crear ProcedureDocument y asociar DocumentId
            var procDoc = new ProcedureDocument
            {
                Name = dto.Name,
                Description = dto.Description,
                Type = dto.Type,
                IsUploaded = true,
                DocumentId = document.Id,
                ProcedureId = dto.ProcedureId
            };

            _context.ProcedureDocuments.Add(procDoc);
            await _context.SaveChangesAsync();

            return procDoc;
        }

        public async Task<ProcedureDocument> GetProcedureDocumentByIdAndUserAsync(int id)
        {
            return await _context.ProcedureDocuments
                .Include(pd => pd.Document)
                .FirstOrDefaultAsync(pd => pd.Id == id);
        }
    }

}
