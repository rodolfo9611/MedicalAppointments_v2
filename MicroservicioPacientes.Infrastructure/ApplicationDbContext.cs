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
    }
}