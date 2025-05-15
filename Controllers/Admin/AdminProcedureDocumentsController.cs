using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using migrapp_api.DTOs.Admin;
using migrapp_api.Services.Admin;

namespace migrapp_api.Controllers.Admin
{
    [ApiController]
    [Route("admin/[controller]")]
    public class AdminProcedureDocumentsController : ControllerBase
    {
        private readonly IAdminProcedureDocumentService _procedureDocumentService;

        public AdminProcedureDocumentsController(IAdminProcedureDocumentService procedureDocumentService)
        {
            _procedureDocumentService = procedureDocumentService;
        }

        // Endpoint para subir documento
        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] UploadProcedureDocumentDto dto)
        {
            try
            {

                var procDoc = await _procedureDocumentService.UploadProcedureDocumentAsync(dto);

                return Ok(new { message = "Documento de procedimiento subido exitosamente", procDoc });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Endpoint para descargar documento de procedimiento
        [HttpGet("download/{id}")]
        public async Task<IActionResult> Download(int id)
        {
            try
            {

                var procDoc = await _procedureDocumentService.GetProcedureDocumentByIdAndUserAsync(id);

                if (procDoc == null || procDoc.Document == null)
                    return NotFound(new { message = "Documento no encontrado o no autorizado." });

                var filePath = procDoc.Document.FilePath;

                if (!System.IO.File.Exists(filePath))
                    return NotFound(new { message = "Archivo físico no encontrado." });

                var memory = new MemoryStream();
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;

                var contentType = GetContentType(filePath);

                return File(memory, contentType, procDoc.Name + Path.GetExtension(filePath));
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
        };

            var ext = Path.GetExtension(path);
            return types.ContainsKey(ext) ? types[ext] : "application/octet-stream";
        }
    }

}
