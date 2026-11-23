using MedicalAppointments.Notifications.Domain.Enums;

namespace MedicalAppointments.Notifications.Application.DTOs;

/// <summary>
/// Datos actualizables de una notificación existente.
/// </summary>
public class UpdateNotificacionDto
{
    public long NotificacionID { get; set; }
    public long CitaID { get; set; }
    public long PacienteID { get; set; }
    public string PacienteNombre { get; set; } = string.Empty;
    public string Destinatario { get; set; } = string.Empty;
    public string Canal { get; set; } = "WhatsApp";
    public string Mensaje { get; set; } = string.Empty;
    public DateTime FechaProgramadaCita { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    public DateTime? FechaEnvio { get; set; }
    public string Estado { get; set; } = "Pendiente";
    public int Intentos { get; set; }
    public string? DetalleRespuesta { get; set; }
    public TipoNotificacionEnum TipoNotificacion { get; set; } = TipoNotificacionEnum.RecordatorioCita;
    public long? RecetaMedicaID { get; set; }
}
