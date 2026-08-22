using MedicalAppointments.Domain.Citas.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedicalAppointments.Persistence.Citas.Context;

public class CitasDbContext : DbContext
{
    public CitasDbContext(DbContextOptions<CitasDbContext> options) : base(options) { }

    public DbSet<Cita> Citas => Set<Cita>();
    public DbSet<EstadoCita> EstadosCita => Set<EstadoCita>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CitasDbContext).Assembly);
    }
}
