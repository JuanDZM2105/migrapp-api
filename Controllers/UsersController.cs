using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using migrapp_api.DTOs;
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

        public IMapper Mapper { get; }

        public UsersController(IRepositorio repositorio,
            IOutputCacheStore outputCacheStore,
            IConfiguration configuration,
            AppDbContext context,
            IMapper mapper
            ) 
        {
            this.repositorio = repositorio;
            this.outputCacheStore = outputCacheStore;
            this.configuration = configuration;
            this.context = context;
            Mapper = mapper;
        }


        [HttpGet("ejemplo-proveedor-config")]
        public string GetEjemploProveedorConfig()
        {

            return configuration.GetValue<string>("AllowedHosts")!;
        }

        [HttpGet]
        [OutputCache(Tags = ["users"])]
        public async Task<List<UserDTO>> Get()
        {

            return await context.Users.ProjectTo<UserDTO>(Mapper.ConfigurationProvider).ToListAsync();
        }

        [HttpGet("{id:int}", Name = "ObtenerUsuarioPorId")]
        [OutputCache(Tags = ["users"])]
        public async Task<ActionResult<UserDTO>> Get(int id)
        {
            var user = await context.Users
                .ProjectTo<UserDTO>(Mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(u => u.id == id);

            if(user is null)
            {
                return NotFound();
            }

            return user;

        }


        [HttpPost()]
        public async Task<IActionResult> Post(int id, [FromBody] UsersCreateDTO UserCreateDTO)
        {
            var user = Mapper.Map<User>(UserCreateDTO);
            context.Add(user);
            await context.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("users", default);   
            return CreatedAtRoute("ObtenerUsuarioPorId", new {id = user.id}, user);

        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] UsersCreateDTO usersCreateDTO)
        {
            var userExists = await context.Users.AnyAsync(u => u.id == id);
            if (!userExists)
            {
                return NotFound();
            }

            var user = Mapper.Map<User>(usersCreateDTO);
            user.id = id;

            context.Update(user);
            await context.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("users", default);

            return NoContent();


        }

        [HttpDelete]
        public void Delete()
        {

        }


    }
}
