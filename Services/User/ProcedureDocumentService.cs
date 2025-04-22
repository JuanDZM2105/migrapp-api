// ProcedureDocumentService.cs
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using migrapp_api.Data;
using migrapp_api.Models;
using migrapp_api.Helpers;        // Aquí está UploadSettings

public class ProcedureDocumentService : IProcedureDocumentService
{
    private readonly ApplicationDbContext _ctx;
    private readonly UploadSettings      _opts;

    public ProcedureDocumentService(
        ApplicationDbContext ctx,
        IOptions<UploadSettings> opts)
    {
        _ctx  = ctx;
        _opts = opts.Value;
    }

    public async Task<Document> UploadAsync(
        int procedureDocumentId,
        IFormFile file,
        int userId)
    {
        // 1. Validar tipo y tamaño
        ValidateFile(file);

        // 2. Cargar ProcedureDocument + verificar dueño
        var procDoc = await _ctx.ProcedureDocuments
            .Include(pd => pd.Procedure)
            .ThenInclude(p => p.LegalProcess)
            .Include(pd => pd.Document)
            .FirstOrDefaultAsync(pd =>
                pd.Id == procedureDocumentId
                && pd.Procedure.LegalProcess.ClientUserId == userId);

        if (procDoc == null)
            throw new KeyNotFoundException("Procedimiento no encontrado o no autorizado.");

        // 3. Si ya existe un Document: bórralo (físico + BD)
        if (procDoc.Document != null)
            await DeletePhysicalFileAndRecord(procDoc.Document);

        // 4. Generar nombre único y guardar a disco
        var ext      = Path.GetExtension(file.FileName);
        var filename = $"{Guid.NewGuid()}{ext}";
        var dir      = Path.Combine(Directory.GetCurrentDirectory(), _opts.StoragePath);
        Directory.CreateDirectory(dir);
        var fullPath = Path.Combine(dir, filename);
        await using (var stream = new FileStream(fullPath, FileMode.Create))
            await file.CopyToAsync(stream);

        // 5. Crear registro en BD
        var doc = new Document {
            Name       = file.FileName,
            Type       = file.ContentType,
            FilePath   = filename,
            UploadedAt = DateTime.UtcNow,
            UserId     = userId
        };
        _ctx.Documents.Add(doc);
        await _ctx.SaveChangesAsync();

        // 6. Actualizar el ProcedureDocument
        procDoc.DocumentId  = doc.Id;
        procDoc.IsUploaded  = true;
        await _ctx.SaveChangesAsync();

        return doc;
    }

    public async Task<FileStreamResult> DownloadAsync(int documentId)
    {
        var doc = await _ctx.Documents.FindAsync(documentId)
                  ?? throw new KeyNotFoundException("Documento no encontrado.");

        var dir      = Path.Combine(Directory.GetCurrentDirectory(), _opts.StoragePath);
        var fullPath = Path.Combine(dir, doc.FilePath);

        if (!File.Exists(fullPath))
            throw new FileNotFoundException("El archivo físico no existe.");

        var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
        return new FileStreamResult(stream, doc.Type) {
            FileDownloadName = doc.Name
        };
    }

    public Task<Document> ReplaceAsync(int procedureDocumentId, IFormFile file, int userId)
    {
        // Simplemente reaprovechamos UploadAsync, que ya borra el anterior
        return UploadAsync(procedureDocumentId, file, userId);
    }

    public async Task DeleteAsync(int procedureDocumentId, int userId)
    {
        var procDoc = await _ctx.ProcedureDocuments
            .Include(pd => pd.Procedure)
            .ThenInclude(p => p.LegalProcess)
            .Include(pd => pd.Document)
            .FirstOrDefaultAsync(pd =>
                pd.Id == procedureDocumentId
                && pd.Procedure.LegalProcess.ClientUserId == userId);

        if (procDoc?.Document == null)
            throw new KeyNotFoundException("No existe documento para eliminar.");

        await DeletePhysicalFileAndRecord(procDoc.Document);

        // Limpiar referencia
        procDoc.DocumentId = null;
        procDoc.IsUploaded = false;
        await _ctx.SaveChangesAsync();
    }

    #region ─── Métodos auxiliares ───

    private void ValidateFile(IFormFile file)
    {
        if (file.Length > _opts.MaxFileSizeBytes)
            throw new InvalidDataException(
                $"Tamaño máximo: {_opts.MaxFileSizeBytes/(1024*1024)} MB.");

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!_opts.AllowedExtensions.Contains(ext))
            throw new InvalidDataException(
                $"Extensión inválida. Permitido: {string.Join(", ", _opts.AllowedExtensions)}");
    }

    private async Task DeletePhysicalFileAndRecord(Document doc)
    {
        var dir      = Path.Combine(Directory.GetCurrentDirectory(), _opts.StoragePath);
        var fullPath = Path.Combine(dir, doc.FilePath);
        if (File.Exists(fullPath)) File.Delete(fullPath);

        _ctx.Documents.Remove(doc);
        await _ctx.SaveChangesAsync();
    }
    #endregion
}
