using Microsoft.AspNetCore.Mvc;
using migrapp_api.Models.HelpCenter;
using migrapp_api.Services.HelpCenter;

namespace migrapp_api.Controllers.User
{
    [ApiController]
    [Route("api/userHelp")]
    public class UserHelpController : ControllerBase
    {
        private readonly IHelpCenterService _helpService;

        public UserHelpController(IHelpCenterService helpService)
        {
            _helpService = helpService;
        }

        [HttpGet]
        public async Task<IActionResult> GetContent([FromQuery] string lang = "en")
        {
            var content = await _helpService.GetHelpCenterContentAsync(lang);
            if (content == null)
                return NotFound(new { error = $"Contenido no disponible para el idioma '{lang}'." });

            return Ok(content);
        }

        [HttpPost("contact")]
        public async Task<IActionResult> SubmitHelpRequest([FromBody] HelpRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Question))
                return BadRequest(new { error = "Faltan datos requeridos" });

            await _helpService.SubmitHelpRequestAsync(request);
            return Ok(new { message = "Tu solicitud ha sido enviada con éxito. Nos pondremos en contacto contigo." });
        }
    }
}
