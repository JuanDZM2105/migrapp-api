using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using migrapp_api.Data;
using migrapp_api.DTOs.Admin;
using migrapp_api.Services.Admin;

namespace migrapp_api.Controllers
{
    [ApiController]
    [Route("api/procedure")]
    [Authorize]
    public class ProcedureController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IProcedureService _procedureService;

        public ProcedureController(ApplicationDbContext context, IProcedureService procedureService)
        {
            _context = context;
            _procedureService = procedureService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProcedure([FromBody] CreateProcedureDto dto)
        {
            try
            {
                // Llamamos al servicio para crear el procedimiento
                var procedure = await _procedureService.CreateProcedureAsync(dto);

                // Retornamos el procedimiento creado
                return Ok(new { message = "proceso legal creado exitosamente" });
            }
            catch (Exception ex)
            {
                // Manejo de errores, si el proceso legal no existe o cualquier otro error
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
