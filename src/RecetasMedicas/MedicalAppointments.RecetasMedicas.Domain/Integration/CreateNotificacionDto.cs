namespace MedicalAppointments.RecetasMedicas.Domain.Integration;

/// <summary>
/// Contrato enviado a POST /api/Notificaciones. Los campos TipoNotificacion y
/// RecetaMedicaID permiten correlacionar el aviso con la receta emitida/anulada.
/// </summary>
public sealed class CreateNotificacionDto
{
    public long CitaID { get; set; }
    public long PacienteID { get; set; }
    public string PacienteNombre { get; set; } = string.Empty;
    public string Destinatario { get; set; } = string.Empty;
    public string Canal { get; set; } = "Email";
    public string Mensaje { get; set; } = string.Empty;
    public DateTime FechaProgramadaCita { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    public string Estado { get; set; } = "Pendiente";
    public string TipoNotificacion { get; set; } = string.Empty;
    public long? RecetaMedicaID { get; set; }
}
