using Microsoft.EntityFrameworkCore;
using migrapp_api.Models;

namespace migrapp_api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<LegalProcess> LegalProcesses { get; set; }
        public DbSet<Procedure> Procedures { get; set; }
        public DbSet<ProcedureDocument> ProcedureDocuments { get; set; }
        public DbSet<AssignedUser> AssignedUsers { get; set; }
        public DbSet<UserLog> UserLogs { get; set; }
        public DbSet<UserMfaCode> UserMfaCodes { get; set; }
        public DbSet<ColumnVisibility> ColumnVisibilities { get; set; }

        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<LegalProcess>()
                .HasOne(lp => lp.ClientUser)
                .WithMany(u => u.ClientLegalProcesses)
                .HasForeignKey(lp => lp.ClientUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LegalProcess>()
                .HasOne(lp => lp.LawyerUser)
                .WithMany(u => u.LawyerLegalProcesses)
                .HasForeignKey(lp => lp.LawyerUserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<LegalProcess>()
                .HasMany(lp => lp.Procedures)
                .WithOne(p => p.LegalProcess)
                .HasForeignKey(p => p.LegalProcessId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Procedure>()
                .HasMany(p => p.ProcedureDocuments)
                .WithOne(pd => pd.Procedure)
                .HasForeignKey(pd => pd.ProcedureId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Document>()
                .HasOne(d => d.User)
                .WithMany(u => u.Documents)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProcedureDocument>()
                .HasOne(pd => pd.Procedure)
                .WithMany(p => p.ProcedureDocuments)
                .HasForeignKey(pd => pd.ProcedureId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AssignedUser>()
                .HasOne(au => au.ClientUser)
                .WithMany(u => u.AssignedProfessionals)
                .HasForeignKey(au => au.ClientUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AssignedUser>()
                .HasOne(au => au.ProfessionalUser)
                .WithMany(u => u.AssignedClients)
                .HasForeignKey(au => au.ProfessionalUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserLog>()
                .HasOne(ul => ul.User)
                .WithMany(u => u.UserLogs)
                .HasForeignKey(ul => ul.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación entre User y ColumnVisibility
            modelBuilder.Entity<ColumnVisibility>()
                .HasOne(cv => cv.User)
                .WithMany() // Un usuario puede tener una configuración de columnas
                .HasForeignKey(cv => cv.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Si el usuario se elimina, se elimina la configuración de columnas
        }
    }
}
