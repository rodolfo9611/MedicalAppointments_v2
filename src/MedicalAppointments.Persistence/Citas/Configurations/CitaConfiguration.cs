using MedicalAppointments.Domain.Citas.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedicalAppointments.Persistence.Citas.Configurations;

public class CitaConfiguration : IEntityTypeConfiguration<Cita>
{
    public void Configure(EntityTypeBuilder<Cita> builder)
    {
        builder.ToTable("Citas");

        builder.HasKey(c => c.CitaID);

        builder.Property(c => c.CitaID)
            .ValueGeneratedOnAdd();

        builder.Property(c => c.PacienteID)
            .IsRequired();

        builder.Property(c => c.MedicoID)
            .IsRequired();

        builder.Property(c => c.EstadoCitaID)
            .IsRequired();

        builder.Property(c => c.FechaHoraInicio)
            .IsRequired();

        builder.Property(c => c.Motivo)
            .HasMaxLength(1000);

        builder.Property(c => c.Observaciones)
            .HasMaxLength(1000);
    }
}
