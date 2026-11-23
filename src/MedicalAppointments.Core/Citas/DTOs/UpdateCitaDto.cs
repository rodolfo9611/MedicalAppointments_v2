using System.ComponentModel.DataAnnotations;
using MedicalAppointments.Domain.Citas.Enums;

namespace MedicalAppointments.Core.Citas.DTOs;

/// <summary>
/// Datos necesarios para registrar una atención clínica dentro del mismo CitaID.
/// </summary>
public class RegistroAtencionDto
{
    [Range(1, int.MaxValue, ErrorMessage = "La secuencia de atención debe ser mayor o igual a 1.")]
    public int Secuencia { get; set; }

    public DateTime? FechaAtencion { get; set; }

    [StringLength(2000, ErrorMessage = "Las observaciones no pueden superar los 2000 caracteres.")]
    public string? Observaciones { get; set; }
}

/// <summary>
/// Reprogramación de una cita en estado PendienteResultados. El CitaID y el estado no cambian.
/// </summary>
public class ReprogramacionDto
{
    [Required(ErrorMessage = "La nueva fecha y hora de atención es obligatoria.")]
    public DateTime NuevaFechaHoraInicio { get; set; }

    public DateTime? NuevaFechaHoraFin { get; set; }

    [Required(ErrorMessage = "El motivo de la reprogramación es obligatorio.")]
    [StringLength(500, ErrorMessage = "El motivo no puede superar los 500 caracteres.")]
    public string Motivo { get; set; } = string.Empty;
}

/// <summary>
/// Solicitud de actualización del proceso clínico de una cita.
/// </summary>
public class UpdateCitaDto
{
    [Required]
    [EnumDataType(typeof(EstadoCitaEnum))]
    public EstadoCitaEnum Estado { get; set; }

    public DateTime? FechaHoraInicio { get; set; }
    public DateTime? FechaHoraFin { get; set; }

    [StringLength(1000)]
    public string? Motivo { get; set; }

    [StringLength(1000)]
    public string? Observaciones { get; set; }

    /// <summary>Obligatorio para registrar una nueva interacción clínica.</summary>
    public RegistroAtencionDto? RegistroAtencion { get; set; }

    /// <summary>Aplicable únicamente cuando el estado actual es PendienteResultados.</summary>
    public ReprogramacionDto? Reprogramacion { get; set; }
}
