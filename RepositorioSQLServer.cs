using migrapp_api.Entidades;

namespace migrapp_api
{
    public class RepositorioSQLServer : IRepositorio
    {
        private List<User> _users;

        public RepositorioSQLServer()
        {
            _users = new List<User>
            {
                new User { id = 1, name = "Juan David Zapata", email = "juandzm2105@gmail.com", phone = "+1 3105149312", password = "ERAS" },
                new User { id = 2, name = "Pedro Rojas Osorio", email = "pedro.rojas@gmail.com", phone = "+1 3117237822", password = "kasjdj" },
                new User { id = 3, name = "Laura Martínez", email = "laura.mtz@example.com", phone = "+1 3001234567", password = "LM2025" },
                new User { id = 4, name = "Carlos Torres", email = "c.torres@example.com", phone = "+1 3019876543", password = "torres321" },
                new User { id = 5, name = "Mariana Gómez", email = "mariana.gomez@example.com", phone = "+57 3021122334", password = "mariana22" },
                new User { id = 6, name = "Andrés López", email = "andres.lopez@example.com", phone = "+57 3104455667", password = "andres123" },
                new User { id = 7, name = "Sofía Ramírez", email = "sofia.ramirez@example.com", phone = "+57 3136677889", password = "SofiaPass" },
                new User { id = 8, name = "Felipe Mejía", email = "felipe.mejia@example.com", phone = "+57 3159988776", password = "felipe!23" },
                new User { id = 9, name = "Daniela Restrepo", email = "daniela.r@example.com", phone = "+1 3123344556", password = "Dani2024" },
                new User { id = 10, name = "Mateo Castaño", email = "mateo.castano@example.com", phone = "+57 3205566778", password = "mateoC" },
                new User { id = 11, name = "Camila Álvarez", email = "camila.a@example.com", phone = "+57 3111122334", password = "camila123" },
                new User { id = 12, name = "Esteban Giraldo", email = "esteban.g@example.com", phone = "+57 3049988776", password = "egpass" },
                new User { id = 13, name = "Valentina Hoyos", email = "valentina.h@example.com", phone = "+57 3054455667", password = "vhoyos" },
                new User { id = 14, name = "Santiago Vélez", email = "santiago.v@example.com", phone = "+57 3102233445", password = "santiv" },
                new User { id = 15, name = "Isabella Muñoz", email = "isabella.m@example.com", phone = "+1 3121234567", password = "isaM!2025" },
                new User { id = 16, name = "Julián Herrera", email = "julian.h@example.com", phone = "+57 3139876543", password = "jhpass2025" },
                new User { id = 17, name = "Natalia Ríos", email = "natalia.r@example.com", phone = "+57 3141122334", password = "natrios" },
                new User { id = 18, name = "Tomás Zuluaga", email = "tomas.z@example.com", phone = "+1 3162233445", password = "tzulu123" },
                new User { id = 19, name = "Gabriela Cano", email = "gabriela.c@example.com", phone = "+57 3173344556", password = "gabi2025" },
                new User { id = 20, name = "Diego Salazar", email = "diego.s@example.com", phone = "+57 3184455667", password = "dsalaz!" },
                new User { id = 21, name = "Paula Cardona", email = "paula.c@example.com", phone = "+57 3195566778", password = "paulita" },
                new User { id = 22, name = "Ricardo Molina", email = "ricardo.m@example.com", phone = "+57 3009988776", password = "rmpass25" },
                new User { id = 23, name = "Ana María Betancur", email = "ana.betancur@example.com", phone = "+57 3016677889", password = "anaMB" },
                new User { id = 24, name = "Sebastián Pérez", email = "sebastian.p@example.com", phone = "+57 3021122334", password = "sebaperez" },
                new User { id = 25, name = "Manuela Londoño", email = "manuela.l@example.com", phone = "+57 3031234567", password = "mlondon" },
                new User { id = 26, name = "Alejandro Vargas", email = "alejandro.v@example.com", phone = "+57 3043344556", password = "avargas" },
                new User { id = 27, name = "Daniel Torres", email = "daniel.t@example.com", phone = "+57 3054455667", password = "dantor" },
                new User { id = 28, name = "Lucía Marín", email = "lucia.marin@example.com", phone = "+57 3065566778", password = "luciaM@1" },
                new User { id = 29, name = "Samuel Gómez", email = "samuel.gomez@example.com", phone = "+57 3076677889", password = "sgomez" },
                new User { id = 30, name = "Carolina Arango", email = "carolina.a@example.com", phone = "+57 3087788990", password = "caroPass" }
            };

        }

        public List<User> ObtenerTodosLosUsuarios()
        {
            return _users;
        }

        public async Task<User?> ObtenerPorId(int Id)
        {
            await Task.Delay(TimeSpan.FromSeconds(3));
            return _users.FirstOrDefault(u => u.id == Id);
        }

        public void Crear(User user)
        {
            _users.Add(user);
        }
    }

}
