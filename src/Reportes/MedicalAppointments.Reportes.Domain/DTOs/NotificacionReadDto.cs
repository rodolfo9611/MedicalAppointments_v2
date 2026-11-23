namespace MedicalAppointments.Reportes.Domain.DTOs;

/// <summary>
/// Proyección de solo lectura de una notificación obtenida por HTTP del microservicio de notificaciones.
/// </summary>
public sealed class NotificacionReadDto
{
    /// <summary>Identificador de la notificación.</summary>
    public long NotificacionID { get; set; }

    /// <summary>Alias del identificador para contratos que exponen <c>Id</c>.</summary>
    public long Id
    {
        get => NotificacionID;
        set => NotificacionID = value;
    }

    /// <summary>Identificador de la cita asociada.</summary>
    public long CitaID { get; set; }

    /// <summary>Identificador del paciente.</summary>
    public long PacienteID { get; set; }

    /// <summary>Nombre del paciente.</summary>
    public string PacienteNombre { get; set; } = string.Empty;

    /// <summary>Tipo de notificación.</summary>
    public string? Tipo { get; set; }

    /// <summary>Alias del tipo para contratos que exponen <c>TipoNotificacion</c>.</summary>
    public string? TipoNotificacion
    {
        get => Tipo;
        set => Tipo = value;
    }

    /// <summary>Estado de la notificación.</summary>
    public string? Estado { get; set; }

    /// <summary>Canal de envío de la notificación.</summary>
    public string? Canal { get; set; }

    /// <summary>Fecha de registro de la notificación.</summary>
    public DateTime FechaRegistro { get; set; }

    /// <summary>Fecha de envío de la notificación.</summary>
    public DateTime? FechaEnvio { get; set; }
}
