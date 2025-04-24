using System;
using System.Linq;
using migrapp_api.Models;
using migrapp_api.Data;

namespace migrapp_api.Data
{
    public class DataSeeder
    {
        private readonly ApplicationDbContext _context;

        public DataSeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Seed()
        {
            // Verifica si ya existe un usuario cliente para evitar duplicados
            if (!_context.Users.Any(u => u.Type == "Client"))
            {
                var clientUser = new User
                {
                    Name = "Juan Diego",
                    LastName = "López",
                    Email = "cliente@example.com",
                    PasswordHash = "hash-seguro", // reemplaza con un hash válido si usas autenticación real
                    PhonePrefix = "+57",
                    Phone = "3000000000",
                    Country = "Colombia",
                    Type = "Client",
                    LastLogin = DateTime.UtcNow
                };

                _context.Users.Add(clientUser);
                _context.SaveChanges();

                var legalProcess = new LegalProcess
                {
                    Name = "Proceso de Residencia",
                    Type = "Migratorio",
                    Status = "Activo",
                    Cost = 3000m,
                    PaymentStatus = "Pagado",
                    StartDate = DateTime.UtcNow,
                    ClientUserId = clientUser.Id
                };

                _context.LegalProcesses.Add(legalProcess);
                _context.SaveChanges();

                var procedure = new Procedure
                {
                    Name = "Entrevista en consulado",
                    Status = "Pendiente",
                    DueDate = DateTime.UtcNow.AddDays(15),
                    LegalProcessId = legalProcess.Id
                };

                _context.Procedures.Add(procedure);
                _context.SaveChanges();
            }
        }
    }
}
