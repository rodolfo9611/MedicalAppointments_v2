using MediatR;
using MedicalAppointments.Reportes.Application.Common;
using MedicalAppointments.Reportes.Application.DTOs;
using MedicalAppointments.Reportes.Domain.Common;
using Nuget.Persistence.Common;

namespace MedicalAppointments.Reportes.Application.Queries.Citas;

/// <summary>
/// Consulta paginada de citas con filtros de estado, paciente y rango de fechas.
/// </summary>
public sealed class GetReporteCitasQuery : PagedQueryBase, IRequest<PagedDto<List<CitaReportDto>>>
{
    /// <summary>Estado de la cita (nombre o valor numérico).</summary>
    public EstadoCitaRead? Estado { get; set; }

    /// <summary>Identificador del paciente.</summary>
    public long? PacienteId { get; set; }

    /// <summary>Fecha y hora inicial del rango.</summary>
    public DateTime? FechaDesde { get; set; }

    /// <summary>Fecha y hora final del rango.</summary>
    public DateTime? FechaHasta { get; set; }
}
