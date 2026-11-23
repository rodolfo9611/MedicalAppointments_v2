namespace MicroservicioPacientes.Application.DTOs;

/// <summary>
/// Representación de un evento del historial clínico del paciente.
/// </summary>
public class GetHistorialMedicoDto
{
    /// <summary>Identificador del evento.</summary>
    public int Id { get; set; }

    /// <summary>Identificador del paciente (alias de PacienteId).</summary>
    public long PacienteID { get; set; }

    /// <summary>Tipo de evento.</summary>
    public string TipoEvento { get; set; } = string.Empty;

    /// <summary>Descripción del evento.</summary>
    public string Descripcion { get; set; } = string.Empty;

    /// <summary>Clave idempotente del evento.</summary>
    public string EventKey { get; set; } = string.Empty;

    /// <summary>Fecha en que ocurrió el evento.</summary>
    public DateTime FechaEvento { get; set; }

    /// <summary>Fecha de registro del evento.</summary>
    public DateTime FechaRegistro { get; set; }

    /// <summary>Cita relacionada, cuando aplica.</summary>
    public long? CitaID { get; set; }

    /// <summary>Estado de cita relacionado, cuando aplica.</summary>
    public int? EstadoCitaID { get; set; }

    /// <summary>Receta médica relacionada, cuando aplica.</summary>
    public long? RecetaMedicaID { get; set; }

    /// <summary>Notificación relacionada, cuando aplica.</summary>
    public long? NotificacionID { get; set; }

    /// <summary>Secuencia de atención registrada, cuando aplica.</summary>
    public int? SecuenciaAtencion { get; set; }

    /// <summary>Fecha anterior a una reprogramación.</summary>
    public DateTime? FechaAnterior { get; set; }

    /// <summary>Nueva fecha tras una reprogramación.</summary>
    public DateTime? FechaNueva { get; set; }

    /// <summary>Motivo de la reprogramación.</summary>
    public string? MotivoReprogramacion { get; set; }

    /// <summary>Diagnóstico asociado al evento.</summary>
    public string Diagnostico { get; set; } = string.Empty;

    /// <summary>Tratamiento asociado al evento.</summary>
    public string Tratamiento { get; set; } = string.Empty;
}
