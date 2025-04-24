using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using migrapp_api.Models;
using migrapp_api.Services;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace migrapp_api.Controllers
{
  [Authorize]
  [ApiController]
  [Route("api/userDocument")]
  public class UserDocumentsController : ControllerBase
  {
    private readonly IUserDocumentService _userDocService;

    public UserDocumentsController(IUserDocumentService userDocService)
    {
      Console.WriteLine("Loaded!");
      _userDocService = userDocService;
    }

    // ✅ 1. Subir documento
    [HttpPost]
    public async Task<ActionResult<Document>> UploadDocument([FromForm] UploadDocumentRequest request)
    {
      int userId = GetUserIdFromToken();
      var uploaded = await _userDocService.UploadAsync(request.File, userId);
      return CreatedAtAction(nameof(GetUserDocuments), new { }, uploaded);
    }


    // ✅ 2. Descargar documento
    [HttpGet("{documentId}")]
    public async Task<ActionResult> DownloadDocument(int documentId)
    {
      int userId = GetUserIdFromToken();
      var file = await _userDocService.DownloadAsync(documentId, userId);
      return file;
    }

    // ✅ 3. Eliminar documento
    [HttpDelete("{documentId}")]
    public async Task<IActionResult> DeleteDocument(int documentId)
    {
      int userId = GetUserIdFromToken();
      await _userDocService.DeleteAsync(documentId, userId);
      return NoContent();
    }

    // ✅ 4. Obtener todos los documentos del usuario
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Document>>> GetUserDocuments()
    {
      int userId = GetUserIdFromToken();
      var documents = await _userDocService.GetUserDocumentsAsync(userId);
      return Ok(documents);
    }

    // 🔐 Método auxiliar para obtener ID de usuario desde el token JWT
    private int GetUserIdFromToken()
    {
      var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
      if (userIdClaim == null)
      {
        throw new UnauthorizedAccessException("No se encontró el ID de usuario en el token.");
      }

      return int.Parse(userIdClaim.Value);
    }


    public class UploadDocumentRequest
    {
      public IFormFile File { get; set; }
    }
  }
}
