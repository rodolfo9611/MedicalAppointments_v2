using MedicalAppointments.Notifications.Application.DTOs;
using MedicalAppointments.Notifications.Domain.Entities;
using MedicalAppointments.Notifications.Domain.Enums;

namespace MedicalAppointments.Notifications.Application.Integration;

/// <summary>
/// Datos mínimos necesarios para consolidar el envío de una notificación en el historial clínico.
/// </summary>
public sealed record NotificacionEnviadaEvent(
    long NotificacionID,
    long CitaID,
    long PacienteID,
    string Estado,
    TipoNotificacionEnum TipoNotificacion,
    string Canal,
    string Destinatario,
    string Mensaje,
    DateTime? FechaEnvio);

public static class NotificacionEnviadaEventExtensions
{
    public static NotificacionEnviadaEvent ToEnviadaEvent(this Notificacion entidad)
        => new(
            entidad.NotificacionID,
            entidad.CitaID,
            entidad.PacienteID,
            entidad.Estado,
            entidad.TipoNotificacion,
            entidad.Canal,
            entidad.Destinatario,
            entidad.Mensaje,
            entidad.FechaEnvio);

    public static NotificacionEnviadaEvent ToEnviadaEvent(this GetNotificacionDto dto)
        => new(
            dto.NotificacionID,
            dto.CitaID,
            dto.PacienteID,
            dto.Estado,
            dto.TipoNotificacion,
            dto.Canal,
            dto.Destinatario,
            dto.Mensaje,
            dto.FechaEnvio);
}
