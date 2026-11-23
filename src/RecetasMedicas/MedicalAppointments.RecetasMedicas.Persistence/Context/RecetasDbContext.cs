using MedicalAppointments.RecetasMedicas.Domain.Entities;
using MedicalAppointments.RecetasMedicas.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace MedicalAppointments.RecetasMedicas.Persistence.Context;

public class RecetasDbContext : DbContext
{
    public RecetasDbContext(DbContextOptions<RecetasDbContext> options)
        : base(options)
    {
    }

    public DbSet<RecetaMedica> RecetasMedicas => Set<RecetaMedica>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var esSqlServer = string.Equals(
            Database.ProviderName,
            "Microsoft.EntityFrameworkCore.SqlServer",
            StringComparison.OrdinalIgnoreCase);

        modelBuilder.ApplyConfiguration(new RecetaMedicaConfiguration(esSqlServer));
    }
}
