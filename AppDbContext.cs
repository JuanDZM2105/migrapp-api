using Microsoft.EntityFrameworkCore;
using migrapp_api.Entidades;

namespace migrapp_api
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
