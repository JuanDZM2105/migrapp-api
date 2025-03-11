using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using migrapp_api.Entidades;
using System.Threading.Tasks;

namespace migrapp_api.Controllers
{
    [Route("api/[Controller]")]
    public class UsersController: ControllerBase
    {
        [HttpGet]
        public List<User> Get()
        {
            var repositorio = new RepositorioEnMemoria();
            var users = repositorio.ObtenerTodosLosUsuarios();
            return users;
        }

        [HttpGet("{id:int}")]
        [OutputCache]
        public async Task<ActionResult<User>> Get(int id)
        {
            var repositorio = new RepositorioEnMemoria();
            var user = await repositorio.ObtenerPorId(id);

            if (user is null)
            {
                return NotFound();
            }

            return user;
        }


        [HttpPost()]
        public void Post(int id, [FromBody] User user)
        {
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
