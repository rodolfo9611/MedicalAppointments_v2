namespace MedicalAppointments.Reportes.Domain.DTOs;

/// <summary>
/// Proyección de solo lectura de una receta médica obtenida por HTTP del microservicio de recetas.
/// </summary>
public sealed class RecetaReadDto
{
    /// <summary>Identificador de la receta.</summary>
    public long RecetaMedicaID { get; set; }

    /// <summary>Identificador de la cita asociada.</summary>
    public long CitaID { get; set; }

    /// <summary>Identificador del paciente.</summary>
    public long PacienteID { get; set; }

    /// <summary>Nombre del paciente.</summary>
    public string NombrePaciente { get; set; } = string.Empty;

    /// <summary>Diagnóstico de la receta.</summary>
    public string Diagnostico { get; set; } = string.Empty;

    /// <summary>Fecha de emisión de la receta.</summary>
    public DateTime FechaEmision { get; set; }

    /// <summary>Indica si la receta fue anulada.</summary>
    public bool EsAnulada { get; set; }
}
