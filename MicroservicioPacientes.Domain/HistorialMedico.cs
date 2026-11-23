namespace MicroservicioPacientes.Domain;

/// <summary>
/// Evento del historial clínico de un paciente. Se conservan los campos originales
/// y se amplía con la información de integración usada por el microservicio de Citas.
/// </summary>
public class HistorialMedico
{
    /// <summary>Identificador del evento.</summary>
    public int Id { get; set; }

    /// <summary>Identificador del paciente propietario del evento.</summary>
    public int PacienteId { get; set; }

    /// <summary>Fecha en que se registró el evento.</summary>
    public DateTime FechaRegistro { get; set; }

    /// <summary>Diagnóstico asociado (compatibilidad con el modelo original).</summary>
    public string Diagnostico { get; set; } = string.Empty;

    /// <summary>Tratamiento asociado (compatibilidad con el modelo original).</summary>
    public string Tratamiento { get; set; } = string.Empty;

    /// <summary>Tipo de evento: CitaCreada, AtencionRegistrada, CambioEstado, Reprogramacion, etc.</summary>
    public string TipoEvento { get; set; } = string.Empty;

    /// <summary>Descripción legible del evento.</summary>
    public string Descripcion { get; set; } = string.Empty;

    /// <summary>Clave idempotente del evento; índice único en base de datos.</summary>
    public string EventKey { get; set; } = string.Empty;

    /// <summary>Fecha en que ocurrió el evento.</summary>
    public DateTime FechaEvento { get; set; }

    /// <summary>Cita relacionada, cuando aplica.</summary>
    public long? CitaID { get; set; }

    /// <summary>Receta médica relacionada, cuando aplica.</summary>
    public long? RecetaMedicaID { get; set; }

    /// <summary>Notificación relacionada, cuando aplica.</summary>
    public long? NotificacionID { get; set; }

    /// <summary>Estado de cita relacionado, cuando aplica.</summary>
    public int? EstadoCitaID { get; set; }

    /// <summary>Secuencia de atención registrada para la cita, cuando aplica.</summary>
    public int? SecuenciaAtencion { get; set; }

    /// <summary>Fecha original antes de una reprogramación.</summary>
    public DateTime? FechaAnterior { get; set; }

    /// <summary>Nueva fecha después de una reprogramación.</summary>
    public DateTime? FechaNueva { get; set; }

    /// <summary>Motivo de la reprogramación, cuando aplica.</summary>
    public string? MotivoReprogramacion { get; set; }
}
