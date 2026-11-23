using MediatR;
using MedicalAppointments.Reportes.Application.DTOs;
using MedicalAppointments.Reportes.Application.Extensions;
using MedicalAppointments.Reportes.Domain.Interfaces;
using MedicalAppointments.Reportes.Domain.Requests;
using Nuget.Persistence.Common;

namespace MedicalAppointments.Reportes.Application.Queries.Citas;

/// <summary>
/// Resuelve el reporte de citas usando exclusivamente el cliente HTTP de citas.
/// </summary>
public sealed class GetReporteCitasQueryHandler : IRequestHandler<GetReporteCitasQuery, PagedDto<List<CitaReportDto>>>
{
    private readonly ICitasApiClient _citasClient;

    /// <summary>Inicializa el handler con el cliente HTTP tipado.</summary>
    public GetReporteCitasQueryHandler(ICitasApiClient citasClient)
    {
        _citasClient = citasClient;
    }

    /// <inheritdoc />
    public async Task<PagedDto<List<CitaReportDto>>> Handle(GetReporteCitasQuery request, CancellationToken cancellationToken)
    {
        var paged = await _citasClient.GetPagedAsync(new CitaFilterRequest
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            EstadoCitaId = request.Estado.HasValue ? (int)request.Estado.Value : null,
            PacienteId = request.PacienteId,
            FechaDesde = request.FechaDesde,
            FechaHasta = request.FechaHasta
        }, cancellationToken);

        var data = paged?.Data?.Select(cita => cita.ToReportDto()).ToList() ?? new List<CitaReportDto>();

        return new PagedDto<List<CitaReportDto>>(
            paged?.TotalRecords ?? data.Count,
            paged?.CurrentPage ?? request.PageNumber,
            paged?.PageSize ?? request.PageSize,
            data);
    }
}
