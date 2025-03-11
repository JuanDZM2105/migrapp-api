using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using migrapp_api.Entidades;
using System.Threading.Tasks;

namespace migrapp_api.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class UsersController: ControllerBase
    {
        private readonly IRepositorio repositorio;
        private readonly IOutputCacheStore outputCacheStore;
        private readonly IConfiguration configuration;
        private readonly AppDbContext context;

        public UsersController(IRepositorio repositorio,
            IOutputCacheStore outputCacheStore,
            IConfiguration configuration,
            AppDbContext context
            ) 
        {
            this.repositorio = repositorio;
            this.outputCacheStore = outputCacheStore;
            this.configuration = configuration;
            this.context = context;
        }


        [HttpGet("ejemplo-proveedor-config")]
        public string GetEjemploProveedorConfig()
        {

            return configuration.GetValue<string>("AllowedHosts")!;
        }

        [HttpGet]
        [OutputCache(Tags = ["users"])]
        public List<User> Get()
        {
            
            var users = repositorio.ObtenerTodosLosUsuarios();
            return users;
        }

        [HttpGet("{id:int}", Name = "ObtenerUsuarioPorId")]
        [OutputCache(Tags = ["users"])]
        public async Task<ActionResult<User>> Get(int id)
        {
            
            var user = await repositorio.ObtenerPorId(id);

            if (user is null)
            {
                return NotFound();
            }

            return user;
        }


        [HttpPost()]
        public async Task<IActionResult> Post(int id, [FromBody] User user)
        {
            context.Add(user);
            await context.SaveChangesAsync();
            return CreatedAtRoute("ObtenerUsuarioPorId", new {id = user.id}, user);

        }

        [HttpPut]
        public void Put()
        {

        }

        [HttpDelete]
        public void Delete()
        {

        }


    }
}
