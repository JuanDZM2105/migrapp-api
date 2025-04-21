using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using migrapp_api.Data;

namespace migrapp_api.Controllers
{
    [ApiController]
    [Route("api/procedure")]
    [Authorize]
    public class ProcedureController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProcedureController(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
