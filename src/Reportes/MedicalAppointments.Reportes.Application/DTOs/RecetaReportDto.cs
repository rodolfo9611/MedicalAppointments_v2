namespace MedicalAppointments.Reportes.Application.DTOs;

/// <summary>Registro de receta devuelto por el reporte de recetas.</summary>
public sealed class RecetaReportDto
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

    /// <summary>Indica si la receta está anulada (siempre falso en los reportes).</summary>
    public bool EsAnulada { get; set; }
}
