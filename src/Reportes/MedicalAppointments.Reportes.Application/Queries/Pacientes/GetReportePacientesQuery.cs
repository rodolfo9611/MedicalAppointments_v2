using MediatR;
using MedicalAppointments.Reportes.Application.Common;
using MedicalAppointments.Reportes.Application.DTOs;
using Nuget.Persistence.Common;

namespace MedicalAppointments.Reportes.Application.Queries.Pacientes;

/// <summary>
/// Consulta paginada de pacientes con filtros de documento, nombre y fecha de nacimiento.
/// </summary>
public sealed class GetReportePacientesQuery : PagedQueryBase, IRequest<PagedDto<List<PacienteReportDto>>>
{
    /// <summary>Documento de identidad del paciente.</summary>
    public string? Documento { get; set; }

    /// <summary>Nombre o apellido del paciente.</summary>
    public string? Nombre { get; set; }

    /// <summary>Fecha inicial del rango de nacimiento.</summary>
    public DateTime? FechaDesde { get; set; }

    /// <summary>Fecha final del rango de nacimiento.</summary>
    public DateTime? FechaHasta { get; set; }
}
