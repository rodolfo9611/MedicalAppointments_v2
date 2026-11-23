using System.ComponentModel.DataAnnotations;

namespace MicroservicioPacientes.Application.DTOs;

/// <summary>
/// Contrato de integración usado por Citas para registrar eventos en el historial
/// clínico de un paciente. Los nombres de propiedades coinciden exactamente con
/// EventoHistorialRequest de MedicalAppointments.Domain.Citas.Integration.
/// </summary>
public class EventoHistorialRequestDto
{
    /// <summary>Identificador del paciente.</summary>
    [Range(1, int.MaxValue)]
    public long PacienteID { get; set; }

    /// <summary>Tipo de evento.</summary>
    [Required(ErrorMessage = "El tipo de evento es obligatorio.")]
    [StringLength(100)]
    public string TipoEvento { get; set; } = string.Empty;

    /// <summary>Descripción del evento.</summary>
    [Required(ErrorMessage = "La descripción es obligatoria.")]
    [StringLength(2000)]
    public string Descripcion { get; set; } = string.Empty;

    /// <summary>Clave idempotente del evento.</summary>
    [Required(ErrorMessage = "La clave del evento es obligatoria.")]
    [StringLength(200)]
    public string EventKey { get; set; } = string.Empty;

    /// <summary>Fecha en que ocurrió el evento.</summary>
    public DateTime? FechaEvento { get; set; }

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
}
