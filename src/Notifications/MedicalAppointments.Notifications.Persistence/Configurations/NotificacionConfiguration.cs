using MedicalAppointments.Notifications.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedicalAppointments.Notifications.Persistence.Configurations;

public class NotificacionConfiguration : IEntityTypeConfiguration<Notificacion>
{
    public void Configure(EntityTypeBuilder<Notificacion> builder)
    {
        builder.ToTable("NotificacionesRecordatorios");

        builder.HasKey(n => n.NotificacionID);

        builder.Property(n => n.NotificacionID)
               .ValueGeneratedOnAdd();

        builder.Property(n => n.CitaID)
               .IsRequired();

        builder.Property(n => n.PacienteNombre)
               .HasMaxLength(150)
               .IsRequired();

        builder.Property(n => n.Destinatario)
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(n => n.Canal)
               .HasMaxLength(30)
               .IsRequired();

        builder.Property(n => n.Mensaje)
               .HasMaxLength(500)
               .IsRequired();

        builder.Property(n => n.Estado)
               .HasMaxLength(30)
               .IsRequired();

        builder.Property(n => n.DetalleRespuesta)
               .HasMaxLength(500);

        builder.Property(n => n.TipoNotificacion)
               .HasConversion<string>()
               .HasMaxLength(30)
               .IsRequired();

        builder.Property(n => n.RecetaMedicaID);

        builder.HasIndex(n => n.CitaID);
        builder.HasIndex(n => n.Estado);
        builder.HasIndex(n => n.TipoNotificacion);
    }
}
