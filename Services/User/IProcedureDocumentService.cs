// IProcedureDocumentService.cs
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using migrapp_api.Models;

public interface IProcedureDocumentService
{
    /// <summary>
    /// Sube un archivo y lo asocia al ProcedureDocument correspondiente.
    /// </summary>
    Task<Document> UploadAsync(int procedureDocumentId, IFormFile file, int userId);

    /// <summary>
    /// Devuelve el FileStreamResult para que el controlador lo envíe al cliente.
    /// </summary>
    Task<FileStreamResult> DownloadAsync(int documentId);

    /// <summary>
    /// Reemplaza (o sube de nuevo) el archivo para un ProcedureDocument existente.
    /// </summary>
    Task<Document> ReplaceAsync(int procedureDocumentId, IFormFile file, int userId);

    /// <summary>
    /// Elimina el archivo y limpia el registro en BD.
    /// </summary>
    Task DeleteAsync(int procedureDocumentId, int userId);
}
