using Microsoft.AspNetCore.Mvc;
using migrapp_api.DTOs.Admin;
using migrapp_api.Services.Admin;

namespace migrapp_api.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        // Endpoint para subir un documento
        [HttpPost("upload")]
        public async Task<IActionResult> UploadDocument([FromForm] UploadDocumentDto dto)
        {
            try
            {
                // Llamamos al servicio para subir el documento
                var document = await _documentService.UploadDocumentAsync(dto);

                // Devolver el documento subido con su información
                return Ok(new { message = "Documento subido con éxito", document });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("user/{userId}/documents")]
        public async Task<IActionResult> GetDocumentsByUser(int userId)
        {
            try
            {
                var documents = await _documentService.GetDocumentsByUserIdExcludingProcedureDocsAsync(userId);
                return Ok(documents);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("download/{documentId}")]
        public async Task<IActionResult> DownloadDocument(int documentId)
        {
            try
            {

                var document = await _documentService.GetDocumentByIdAndUserIdAsync(documentId);

                if (document == null)
                    return NotFound(new { message = "Documento no encontrado o no autorizado." });

                var filePath = document.FilePath;

                if (!System.IO.File.Exists(filePath))
                    return NotFound(new { message = "Archivo físico no encontrado." });

                var fileInfo = new FileInfo(filePath);
                if (fileInfo.Length == 0)
                    return NotFound(new { message = "Archivo vacío" });
                
                var memory = new MemoryStream();
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;

                // Obtener el tipo MIME (content type) según extensión
                var contentType = GetContentType(filePath);

                // Retornar archivo para descarga
                return File(memory, contentType, document.Name + Path.GetExtension(filePath));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private string GetContentType(string path)
        {
            var types = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
    {
        {".pdf", "application/pdf"},
        {".txt", "text/plain"},
        {".doc", "application/msword"},
        {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
        {".xls", "application/vnd.ms-excel"},
        {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
        {".png", "image/png"},
        {".jpg", "image/jpeg"},
        {".jpeg", "image/jpeg"},
        {".gif", "image/gif"},
        // agrega más tipos según necesites
    };

            var ext = Path.GetExtension(path);
            return types.ContainsKey(ext) ? types[ext] : "application/octet-stream";
        }



    }

}
