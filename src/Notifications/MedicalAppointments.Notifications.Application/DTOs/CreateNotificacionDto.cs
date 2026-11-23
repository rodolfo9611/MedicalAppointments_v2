using MedicalAppointments.Notifications.Domain.Enums;

namespace MedicalAppointments.Notifications.Application.DTOs;

/// <summary>
/// Datos necesarios para crear una notificación. Los nombres coinciden con la entidad
/// para conservar compatibilidad con clientes que enviaban <c>Notificacion</c>.
/// </summary>
public class CreateNotificacionDto
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
