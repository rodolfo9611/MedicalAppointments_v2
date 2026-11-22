namespace MedicalAppointments.Notifications.Domain.Entities;

public class Notificacion
{
    public long NotificacionID { get; set; }
    public long CitaID { get; set; }
    public long PacienteID { get; set; }
    public string PacienteNombre { get; set; } = string.Empty;
    public string Destinatario { get; set; } = string.Empty; // Teléfono o Email
    public string Canal { get; set; } = "WhatsApp"; // SMS, WhatsApp, Email
    public string Mensaje { get; set; } = string.Empty;
    public DateTime FechaProgramadaCita { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    public DateTime? FechaEnvio { get; set; }
    public string Estado { get; set; } = "Pendiente"; // Pendiente, Enviado, Fallido
    public int Intentos { get; set; } = 0;
    public string? DetalleRespuesta { get; set; }
}
