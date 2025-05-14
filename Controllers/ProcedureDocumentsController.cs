using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using migrapp_api.Services.User;
using System.Threading.Tasks;
using System.Security.Claims;
using migrapp_api.Services.Admin;
using migrapp_api.Helpers.Admin;

[ApiController]
[Route("api/procedures/{procedureId}/documents")]
[Authorize(Roles = "Client")] // Solo clientes pueden acceder
public class ProcedureDocumentsController : ControllerBase
{
  private readonly IProcedureDocumentService _documentService;
  private readonly ILogService _logService;
  private readonly IHttpContextAccessor _httpContextAccessor;

    public ProcedureDocumentsController(
     IProcedureDocumentService documentService,
     ILogService logService,
    IHttpContextAccessor httpContextAccessor)
  {
    _documentService = documentService;
        _logService = logService;
        _httpContextAccessor = httpContextAccessor;
    }

  // POST: subir documento
  [HttpPost("{procedureDocumentId}/upload")]
  public async Task<IActionResult> Upload(int procedureId, int procedureDocumentId, IFormFile file)
  {
    try
    {
      var userId = GetUserId();

      var doc = await _documentService.UploadAsync(procedureDocumentId, file, userId);

      string ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
      await _logService.LogActionAsync(
        userId,
        LogActionTypes.Create,
        $"Documento de procedimiento subido: {file.FileName}, ID del procedimiento: {procedureId}, ID del documento de procedimiento: {procedureDocumentId}",
        ipAddress);

            return Ok(new
      {
        doc.Id,
        doc.Name,
        doc.UploadedAt
      });
    }
    catch (Exception ex)
    {
      return BadRequest(new { error = ex.Message });
    }
  }

  // GET: descargar documento
  [HttpGet("{procedureDocumentId}/download")]
  public async Task<IActionResult> Download(int procedureId, int procedureDocumentId)
  {
    try
    {
      var userId = GetUserId();

      string ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
      await _logService.LogActionAsync(
        userId,
        LogActionTypes.Download,
        $"Documento de procedimiento descargado, ID del procedimiento: {procedureId}, ID del documento de procedimiento: {procedureDocumentId}",
        ipAddress);

            // Necesitamos cargar el ID del documento desde ProcedureDocument (opcional si ya lo sabes)
            var streamResult = await _documentService.DownloadAsync(procedureDocumentId);
      return streamResult;
    }
    catch (Exception ex)
    {
      return NotFound(new { error = ex.Message });
    }
  }

  // PUT: reemplazar documento
  [HttpPut("{procedureDocumentId}/replace")]
  public async Task<IActionResult> Replace(int procedureId, int procedureDocumentId, IFormFile file)
  {
    try
    {
      var userId = GetUserId();
      var doc = await _documentService.ReplaceAsync(procedureDocumentId, file, userId);

      string ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
      await _logService.LogActionAsync(
        userId,
        LogActionTypes.Update,
        $"Documento de procedimiento reemplazado: {file.FileName}, ID del procedimiento: {procedureId}, ID del documento de procedimiento: {procedureDocumentId}",
        ipAddress);

            return Ok(new
      {
        doc.Id,
        doc.Name,
        doc.UploadedAt
      });
    }
    catch (Exception ex)
    {
      return BadRequest(new { error = ex.Message });
    }
  }

  // DELETE: eliminar documento
  [HttpDelete("{procedureDocumentId}")]
  public async Task<IActionResult> Delete(int procedureId, int procedureDocumentId)
  {
    try
    {
      var userId = GetUserId();
      await _documentService.DeleteAsync(procedureDocumentId, userId);

      string ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
      await _logService.LogActionAsync(
       userId, 
       LogActionTypes.Delete,
       $"Documento de procedimiento eliminado, ID del procedimiento: {procedureId}, ID del documento de procedimiento: {procedureDocumentId}",
       ipAddress);

            return NoContent();
    }
    catch (Exception ex)
    {
      return NotFound(new { error = ex.Message });
    }
  }

  // Extrae el ID del usuario autenticado desde el token JWT
  private int GetUserId()
  {
    var userIdClaim = User?.FindFirst(ClaimTypes.NameIdentifier) ?? User?.FindFirst("sub");
    if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
      throw new UnauthorizedAccessException("Usuario no autenticado.");

    return userId;
  }
}
