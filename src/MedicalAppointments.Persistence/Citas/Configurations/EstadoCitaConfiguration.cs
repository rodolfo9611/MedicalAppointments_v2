using MedicalAppointments.Domain.Citas.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedicalAppointments.Persistence.Citas.Configurations;

public class EstadoCitaConfiguration : IEntityTypeConfiguration<EstadoCita>
{
    public void Configure(EntityTypeBuilder<EstadoCita> builder)
    {
        builder.ToTable("Estados_Cita");

        builder.HasKey(e => e.EstadoCitaID);

        builder.Property(e => e.EstadoCitaID)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Codigo)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(e => e.Nombre)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(e => e.Codigo)
            .IsUnique();
    }
}
