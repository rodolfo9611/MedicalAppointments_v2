using System.ComponentModel.DataAnnotations;

namespace MedicalAppointments.RecetasMedicas.Api.Requests;

/// <summary>
/// Cuerpo permitido para emitir una receta. NO incluye paciente, cita,
/// médico ni fecha: se obtienen de los microservicios y del servidor.
/// </summary>
public sealed class CreateRecetaMedicaRequest
{
    /// <summary>Diagnóstico clínico de la consulta.</summary>
    [Required]
    [MaxLength(2000)]
    public string Diagnostico { get; set; } = string.Empty;

    /// <summary>Indicaciones generales para el paciente.</summary>
    [Required]
    [MaxLength(4000)]
    public string Indicaciones { get; set; } = string.Empty;

    /// <summary>Tratamiento prescrito.</summary>
    [Required]
    [MaxLength(4000)]
    public string Tratamiento { get; set; } = string.Empty;

    /// <summary>Observaciones opcionales.</summary>
    [MaxLength(2000)]
    public string? Observaciones { get; set; }
}
