using Microsoft.EntityFrameworkCore;
using MicroservicioPacientes.Domain;

namespace MicroservicioPacientes.Infrastructure;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Paciente> Pacientes => Set<Paciente>();
    public DbSet<HistorialMedico> HistorialesMedicos => Set<HistorialMedico>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de relación 1 a Muchos
        modelBuilder.Entity<Paciente>()
            .HasMany(p => p.Historiales)
            .WithOne()
            .HasForeignKey(h => h.PacienteId);

        modelBuilder.Entity<Paciente>(entity =>
        {
            entity.Property(p => p.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(p => p.Apellido).IsRequired().HasMaxLength(100);
            entity.Property(p => p.DocumentoIdentidad).IsRequired().HasMaxLength(30);
            entity.Property(p => p.CodigoPaciente).IsRequired().HasMaxLength(30);
            entity.Property(p => p.CorreoElectronico).HasMaxLength(150);
            entity.Property(p => p.Telefono).HasMaxLength(30);

            // Código de paciente único en base de datos.
            entity.HasIndex(p => p.CodigoPaciente).IsUnique();
        });

        modelBuilder.Entity<HistorialMedico>(entity =>
        {
            entity.Property(h => h.TipoEvento).IsRequired().HasMaxLength(100);
            entity.Property(h => h.Descripcion).IsRequired().HasMaxLength(2000);
            entity.Property(h => h.Diagnostico).HasMaxLength(2000);
            entity.Property(h => h.Tratamiento).HasMaxLength(2000);
            entity.Property(h => h.EventKey).IsRequired().HasMaxLength(200);
            entity.Property(h => h.MotivoReprogramacion).HasMaxLength(500);

            // Idempotencia de eventos: un EventKey sólo puede registrarse una vez.
            entity.HasIndex(h => h.EventKey).IsUnique();

            entity.HasIndex(h => new { h.CitaID, h.TipoEvento });
        });
    }
}
