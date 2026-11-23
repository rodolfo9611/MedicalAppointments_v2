using Nuget.Persistence.Common;

namespace MedicalAppointments.Reportes.Domain.Requests;

/// <summary>
/// Filtros enviados al cliente HTTP del microservicio de recetas médicas.
/// </summary>
public sealed class RecetaFilterRequest : RequestParametersGets
{
    /// <summary>Identificador del paciente.</summary>
    public long? PacienteId { get; set; }

    /// <summary>Diagnóstico a buscar.</summary>
    public string? Diagnostico { get; set; }

    /// <summary>Fecha inicial del rango de emisión.</summary>
    public DateTime? FechaDesde { get; set; }

    /// <summary>Fecha final del rango de emisión.</summary>
    public DateTime? FechaHasta { get; set; }

    /// <summary>
    /// Indica si se incluyen recetas anuladas. Los reportes siempre lo envían en <c>false</c>.
    /// </summary>
    public bool IncluirAnuladas { get; set; } = false;
}
