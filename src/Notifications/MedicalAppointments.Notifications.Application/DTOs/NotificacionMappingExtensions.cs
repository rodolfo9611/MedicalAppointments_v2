using MedicalAppointments.Notifications.Domain.Entities;

namespace MedicalAppointments.Notifications.Application.DTOs;

public static class NotificacionMappingExtensions
{
    public static Notificacion ToEntity(this CreateNotificacionDto dto)
    {
        return new Notificacion
        {
            CitaID = dto.CitaID,
            PacienteID = dto.PacienteID,
            PacienteNombre = dto.PacienteNombre,
            Destinatario = dto.Destinatario,
            Canal = string.IsNullOrWhiteSpace(dto.Canal) ? "WhatsApp" : dto.Canal,
            Mensaje = dto.Mensaje,
            FechaProgramadaCita = dto.FechaProgramadaCita,
            FechaRegistro = DateTime.UtcNow,
            FechaEnvio = dto.FechaEnvio,
            Estado = string.IsNullOrWhiteSpace(dto.Estado) ? "Pendiente" : dto.Estado,
            Intentos = dto.Intentos,
            DetalleRespuesta = dto.DetalleRespuesta,
            TipoNotificacion = dto.TipoNotificacion,
            RecetaMedicaID = dto.RecetaMedicaID
        };
    }

    public static void ApplyTo(this UpdateNotificacionDto dto, Notificacion entidad)
    {
        entidad.CitaID = dto.CitaID;
        entidad.PacienteID = dto.PacienteID;
        entidad.PacienteNombre = dto.PacienteNombre;
        entidad.Destinatario = dto.Destinatario;
        entidad.Canal = string.IsNullOrWhiteSpace(dto.Canal) ? "WhatsApp" : dto.Canal;
        entidad.Mensaje = dto.Mensaje;
        entidad.FechaProgramadaCita = dto.FechaProgramadaCita;
        entidad.FechaRegistro = dto.FechaRegistro;
        entidad.FechaEnvio = dto.FechaEnvio;
        entidad.Estado = string.IsNullOrWhiteSpace(dto.Estado) ? "Pendiente" : dto.Estado;
        entidad.Intentos = dto.Intentos;
        entidad.DetalleRespuesta = dto.DetalleRespuesta;
        entidad.TipoNotificacion = dto.TipoNotificacion;
        entidad.RecetaMedicaID = dto.RecetaMedicaID;
    }

    public static GetNotificacionDto ToDto(this Notificacion entidad)
    {
        return new GetNotificacionDto
        {
            NotificacionID = entidad.NotificacionID,
            CitaID = entidad.CitaID,
            PacienteID = entidad.PacienteID,
            PacienteNombre = entidad.PacienteNombre,
            Destinatario = entidad.Destinatario,
            Canal = entidad.Canal,
            Mensaje = entidad.Mensaje,
            FechaProgramadaCita = entidad.FechaProgramadaCita,
            FechaRegistro = entidad.FechaRegistro,
            FechaEnvio = entidad.FechaEnvio,
            Estado = entidad.Estado,
            Intentos = entidad.Intentos,
            DetalleRespuesta = entidad.DetalleRespuesta,
            TipoNotificacion = entidad.TipoNotificacion,
            RecetaMedicaID = entidad.RecetaMedicaID
        };
    }
}
