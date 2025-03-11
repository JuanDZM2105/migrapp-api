using migrapp_api.Entidades;

namespace migrapp_api
{
    public interface IRepositorio
    {
        List<User> ObtenerTodosLosUsuarios();
        Task<User> ObtenerPorId(int id);
        void Crear(User user);
    }
}
