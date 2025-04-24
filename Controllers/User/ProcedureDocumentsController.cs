using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using migrapp_api.Services.User;
using System.Threading.Tasks;
using System.Security.Claims;

[ApiController]
[Route("api/procedures/{procedureId}/documents")]
[Authorize(Roles = "Client")] // Solo clientes pueden acceder
public class ProcedureDocumentsController : ControllerBase
{
  private readonly IProcedureDocumentService _documentService;

  public ProcedureDocumentsController(IProcedureDocumentService documentService)
  {
    _documentService = documentService;
  }

  // POST: subir documento
  [HttpPost("{procedureDocumentId}/upload")]
  public async Task<IActionResult> Upload(int procedureId, int procedureDocumentId, IFormFile file)
  {
    try
    {
      var userId = GetUserId();

      var doc = await _documentService.UploadAsync(procedureDocumentId, file, userId);

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
