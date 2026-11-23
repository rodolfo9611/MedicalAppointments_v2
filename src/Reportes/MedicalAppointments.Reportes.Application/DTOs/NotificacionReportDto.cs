namespace MedicalAppointments.Reportes.Application.DTOs;

/// <summary>Registro de notificación devuelto por el reporte de notificaciones.</summary>
public sealed class NotificacionReportDto
{
    /// <summary>Identificador de la notificación.</summary>
    public long NotificacionID { get; set; }

    /// <summary>Identificador de la cita asociada.</summary>
    public long CitaID { get; set; }

    /// <summary>Identificador del paciente.</summary>
    public long PacienteID { get; set; }

    /// <summary>Nombre del paciente.</summary>
    public string PacienteNombre { get; set; } = string.Empty;

    /// <summary>Tipo de notificación.</summary>
    public string? Tipo { get; set; }

    /// <summary>Estado de la notificación.</summary>
    public string? Estado { get; set; }

    /// <summary>Canal de la notificación.</summary>
    public string? Canal { get; set; }

    /// <summary>Fecha de registro de la notificación.</summary>
    public DateTime FechaRegistro { get; set; }

    /// <summary>Fecha de envío de la notificación.</summary>
    public DateTime? FechaEnvio { get; set; }
}
