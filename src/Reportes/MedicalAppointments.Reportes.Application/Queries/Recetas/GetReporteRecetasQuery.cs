using MediatR;
using MedicalAppointments.Reportes.Application.Common;
using MedicalAppointments.Reportes.Application.DTOs;
using Nuget.Persistence.Common;

namespace MedicalAppointments.Reportes.Application.Queries.Recetas;

/// <summary>
/// Consulta paginada de recetas médicas vigentes (nunca incluye anuladas).
/// </summary>
public sealed class GetReporteRecetasQuery : PagedQueryBase, IRequest<PagedDto<List<RecetaReportDto>>>
{
    /// <summary>Identificador del paciente.</summary>
    public long? PacienteId { get; set; }

    /// <summary>Diagnóstico a buscar.</summary>
    public string? Diagnostico { get; set; }

    /// <summary>Fecha inicial del rango de emisión.</summary>
    public DateTime? FechaDesde { get; set; }

    /// <summary>Fecha final del rango de emisión.</summary>
    public DateTime? FechaHasta { get; set; }
}
