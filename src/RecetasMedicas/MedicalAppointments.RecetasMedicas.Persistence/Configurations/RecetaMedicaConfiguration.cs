using MedicalAppointments.RecetasMedicas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedicalAppointments.RecetasMedicas.Persistence.Configurations;

public class RecetaMedicaConfiguration : IEntityTypeConfiguration<RecetaMedica>
{
    private readonly bool _usaIndiceFiltradoSqlServer;

    /// <param name="usaIndiceFiltradoSqlServer">
    /// SQL Server admite índice único filtrado por EsAnulada = 0; InMemory usa
    /// índice normal y la unicidad de la receta activa se valida en el handler.
    /// </param>
    public RecetaMedicaConfiguration(bool usaIndiceFiltradoSqlServer = false)
    {
        _usaIndiceFiltradoSqlServer = usaIndiceFiltradoSqlServer;
    }

    public void Configure(EntityTypeBuilder<RecetaMedica> builder)
    {
        builder.ToTable("RecetasMedicas");
        builder.HasKey(r => r.RecetaMedicaID);
        builder.Property(r => r.RecetaMedicaID).ValueGeneratedOnAdd();

        builder.Property(r => r.CitaID).IsRequired();
        builder.Property(r => r.PacienteID).IsRequired();
        builder.Property(r => r.NombrePaciente).HasMaxLength(200).IsRequired();
        builder.Property(r => r.FechaConsulta).IsRequired();

        builder.Property(r => r.MedicoID);
        builder.Property(r => r.NombreMedico).HasMaxLength(200);
        builder.Property(r => r.Especialidad).HasMaxLength(150);

        builder.Property(r => r.Diagnostico).HasMaxLength(2000).IsRequired();
        builder.Property(r => r.Indicaciones).HasMaxLength(4000).IsRequired();
        builder.Property(r => r.Tratamiento).HasMaxLength(4000).IsRequired();
        builder.Property(r => r.Observaciones).HasMaxLength(2000);
        builder.Property(r => r.FechaEmision).IsRequired();
        builder.Property(r => r.EsAnulada).IsRequired();
        builder.Property(r => r.FechaAnulacion);
        builder.Property(r => r.MotivoAnulacion).HasMaxLength(500);
        builder.Property(r => r.RecetaAnteriorID);

        if (_usaIndiceFiltradoSqlServer)
        {
            builder.HasIndex(r => r.CitaID)
                   .IsUnique()
                   .HasFilter("[EsAnulada] = 0")
                   .HasDatabaseName("IX_RecetasMedicas_CitaID_Activa");
        }
        else
        {
            builder.HasIndex(r => r.CitaID)
                   .HasDatabaseName("IX_RecetasMedicas_CitaID");
        }

        builder.HasIndex(r => new { r.PacienteID, r.FechaEmision })
               .HasDatabaseName("IX_RecetasMedicas_PacienteID_FechaEmision");

        builder.HasIndex(r => r.RecetaAnteriorID)
               .HasDatabaseName("IX_RecetasMedicas_RecetaAnteriorID");
    }
}
