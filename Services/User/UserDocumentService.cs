// UserDocumentService.cs
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using migrapp_api.Data;
using migrapp_api.Helpers;
using migrapp_api.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class UserDocumentService : IUserDocumentService
{
  private readonly ApplicationDbContext _ctx;
  private readonly UploadSettings _opts;

  public UserDocumentService(ApplicationDbContext ctx, IOptions<UploadSettings> opts)
  {
    _ctx = ctx;
    _opts = opts.Value;
  }

  public async Task<Document> UploadAsync(IFormFile file, int userId)
  {
    ValidateFile(file);

    var ext = Path.GetExtension(file.FileName);
    var filename = $"{Guid.NewGuid()}{ext}";
    var dir = Path.Combine(Directory.GetCurrentDirectory(), _opts.StoragePath);
    Directory.CreateDirectory(dir);
    var fullPath = Path.Combine(dir, filename);

    await using (var stream = new FileStream(fullPath, FileMode.Create))
      await file.CopyToAsync(stream);

    var doc = new Document
    {
      Name = file.FileName,
      Type = file.ContentType,
      FilePath = filename,
      UploadedAt = DateTime.UtcNow,
      UserId = userId
    };

    _ctx.Documents.Add(doc);
    await _ctx.SaveChangesAsync();

    return doc;
  }

  public async Task<FileStreamResult> DownloadAsync(int documentId, int userId)
  {
    var doc = await _ctx.Documents.FirstOrDefaultAsync(d => d.Id == documentId && d.UserId == userId)
              ?? throw new UnauthorizedAccessException("No tienes acceso a este archivo");

    var dir = Path.Combine(Directory.GetCurrentDirectory(), _opts.StoragePath);
    var fullPath = Path.Combine(dir, doc.FilePath);

    if (!File.Exists(fullPath))
      throw new FileNotFoundException("Archivo no encontrado en disco");

    var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
    return new FileStreamResult(stream, doc.Type) { FileDownloadName = doc.Name };
  }

  public async Task DeleteAsync(int documentId, int userId)
  {
    var doc = await _ctx.Documents.FirstOrDefaultAsync(d => d.Id == documentId && d.UserId == userId)
              ?? throw new KeyNotFoundException("Documento no encontrado o no autorizado");

    var fullPath = Path.Combine(Directory.GetCurrentDirectory(), _opts.StoragePath, doc.FilePath);
    if (File.Exists(fullPath))
      File.Delete(fullPath);

    _ctx.Documents.Remove(doc);
    await _ctx.SaveChangesAsync();
  }

  public async Task<IEnumerable<Document>> GetUserDocumentsAsync(int userId)
  {
    return await _ctx.Documents
        .Where(d => d.UserId == userId)
        .OrderByDescending(d => d.UploadedAt)
        .ToListAsync();
  }

  private void ValidateFile(IFormFile file)
  {
    if (file.Length > _opts.MaxFileSizeBytes)
      throw new InvalidDataException($"Máximo permitido: {_opts.MaxFileSizeBytes / (1024 * 1024)} MB.");

    var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
    if (!_opts.AllowedExtensions.Contains(ext))
      throw new InvalidDataException($"Extensión no permitida. Solo: {string.Join(", ", _opts.AllowedExtensions)}");
  }
}
