using Bogus;
using Microsoft.EntityFrameworkCore;
using migrapp_api.Data;
using migrapp_api.Entidades;

namespace migrapp_api.Seeding
{
    public static class DbSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (context.Users.Any()) return;

            var faker = new Faker();
            var users = new List<User>();

            var userFaker = new Faker<User>()
                .RuleFor(u => u.Name, f => f.Name.FirstName())
                .RuleFor(u => u.LastName, f => f.Name.LastName())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.PasswordHash, f => f.Internet.Password())
                .RuleFor(u => u.PhonePrefix, f => f.PickRandom("+1", "+57", "+34"))
                .RuleFor(u => u.Phone, f => f.Phone.PhoneNumber("###-###-####"))
                .RuleFor(u => u.Country, f => f.Address.Country())
                .RuleFor(u => u.BirthDate, f => f.Date.Past(30, DateTime.Today.AddYears(-18)))
                .RuleFor(u => u.AccountCreated, f => f.Date.Past(2))
                .RuleFor(u => u.AccountStatus, f => f.PickRandom("active", "eliminated", "blocked"))
                .RuleFor(u => u.LastLogin, f => f.Date.Recent(30))
                .RuleFor(u => u.IsActiveNow, f => f.Random.Bool())
                .RuleFor(u => u.Documents, _ => new List<Document>())
                .RuleFor(u => u.ClientLegalProcesses, _ => new List<LegalProcess>())
                .RuleFor(u => u.LawyerLegalProcesses, _ => new List<LegalProcess>())
                .RuleFor(u => u.AssignedProfessionals, _ => new List<AssignedUser>())
                .RuleFor(u => u.AssignedClients, _ => new List<AssignedUser>())
                .RuleFor(u => u.UserLogs, _ => new List<UserLog>());

            // Usuarios por tipo controlado
            users.AddRange(userFaker.Clone().RuleFor(u => u.UserType, _ => "user").Generate(30));
            users.AddRange(userFaker.Clone().RuleFor(u => u.UserType, _ => "admin").Generate(5));
            users.AddRange(userFaker.Clone().RuleFor(u => u.UserType, _ => "lawyer").Generate(7));
            users.AddRange(userFaker.Clone().RuleFor(u => u.UserType, _ => "auditor").Generate(4));
            users.AddRange(userFaker.Clone().RuleFor(u => u.UserType, _ => "reader").Generate(4));

            context.Users.AddRange(users);
            context.SaveChanges();

            // DOCUMENTS
            var documents = new Faker<Document>()
                .RuleFor(d => d.DocumentType, f => f.PickRandom("ID Card", "Driver License", "Contract", "Proof of Residence", "Court Notice", "Medical Certificate", "Birth Certificate"))
                .RuleFor(d => d.FilePath, f => $"/docs/user_{f.Random.Number(1, 50)}/{f.System.FileName("pdf")}")
                .RuleFor(d => d.UploadedAt, f => f.Date.Past(1))
                .RuleFor(d => d.UserId, f => f.PickRandom(users.Select(u => u.UserId)))
                .Generate(50);

            context.Documents.AddRange(documents);
            context.SaveChanges();

            // LEGAL PROCESSES
            var legalProcesses = new Faker<LegalProcess>()
                .RuleFor(p => p.ProcessType, f => f.PickRandom("Divorce", "Custody", "Civil lawsuit", "Labor dispute", "Inheritance", "Business contract", "Immigration"))
                .RuleFor(p => p.ProcessStatus, f => f.PickRandom("open", "in progress", "paused", "closed", "cancelled"))
                .RuleFor(p => p.Cost, f => f.Random.Decimal(1000, 10000))
                .RuleFor(p => p.PaymentStatus, f => f.PickRandom("paid", "pending", "overdue"))
                .RuleFor(p => p.StartDate, f => f.Date.Past(2))
                .RuleFor(p => p.EndDate, (f, p) => (p.ProcessStatus == "closed" || p.ProcessStatus == "cancelled") ? f.Date.Between(p.StartDate, DateTime.Now) : null)
                .RuleFor(p => p.ClientUserId, f => f.PickRandom(users.Where(u => u.UserType == "user").Select(u => u.UserId)))
                .RuleFor(p => p.LawyerUserId, f => f.PickRandom(users.Where(u => u.UserType == "lawyer").Select(u => u.UserId)))
                .RuleFor(p => p.RequiredDocuments, _ => new List<LegalProcessDocument>())
                .Generate(50);

            context.LegalProcesses.AddRange(legalProcesses);
            context.SaveChanges();

            // ASSIGNED USERS
            var assignedUsers = new Faker<AssignedUser>()
                .RuleFor(a => a.ClientUserId, f => f.PickRandom(users.Where(u => u.UserType == "user").Select(u => u.UserId)))
                .RuleFor(a => a.ProfessionalUserId, f => f.PickRandom(users.Where(u => u.UserType == "reader" || u.UserType == "auditor" || u.UserType == "lawyer").Select(u => u.UserId)))
                .RuleFor(a => a.ProfessionalRole, f => f.PickRandom("reader", "auditor", "lawyer"))
                .RuleFor(a => a.AssignedAt, f => f.Date.Recent(60))
                .Generate(50);

            context.AssignedUsers.AddRange(assignedUsers);
            context.SaveChanges();

            // LEGAL PROCESS DOCUMENTS
            var legalProcessDocuments = new List<LegalProcessDocument>();
            for (int i = 0; i < 50; i++)
            {
                var isUploaded = faker.Random.Bool();

                legalProcessDocuments.Add(new LegalProcessDocument
                {
                    RequiredDocumentType = faker.PickRandom("Contract", "ID Proof", "Financial Statement", "Medical Report", "Evidence File", "Witness Statement"),
                    IsUploaded = isUploaded,
                    DocumentId = isUploaded ? faker.PickRandom(documents.Select(d => d.DocumentId)) : null,
                    LegalProcessId = faker.PickRandom(legalProcesses.Select(lp => lp.LegalProcessId))
                });
            }

            context.LegalProcessDocuments.AddRange(legalProcessDocuments);
            context.SaveChanges();

            // USER LOGS
            var userLogs = new Faker<UserLog>()
                .RuleFor(l => l.UserId, f => f.PickRandom(users.Select(u => u.UserId)))
                .RuleFor(l => l.ActionType, f => f.PickRandom("Login", "Logout", "Create", "Update", "Delete", "Password Change"))
                .RuleFor(l => l.Description, f => f.Lorem.Sentence())
                .RuleFor(l => l.IpAddress, f => f.Internet.Ip())
                .RuleFor(l => l.ActionDate, f => f.Date.Recent(90))
                .Generate(50);

            context.UserLogs.AddRange(userLogs);
            context.SaveChanges();
        }
    }
}

