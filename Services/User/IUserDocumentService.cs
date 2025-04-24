// IUserDocumentService.cs
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using migrapp_api.Models;

public interface IUserDocumentService
{
  Task<Document> UploadAsync(IFormFile file, int userId);
  Task<FileStreamResult> DownloadAsync(int documentId, int userId);
  Task DeleteAsync(int documentId, int userId);
  Task<IEnumerable<Document>> GetUserDocumentsAsync(int userId);
}
