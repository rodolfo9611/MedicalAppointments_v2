using MediatR;
using MedicalAppointments.Reportes.Application.DTOs;
using MedicalAppointments.Reportes.Application.Extensions;
using MedicalAppointments.Reportes.Domain.Interfaces;
using MedicalAppointments.Reportes.Domain.Requests;
using Nuget.Persistence.Common;

namespace MedicalAppointments.Reportes.Application.Queries.Pacientes;

/// <summary>
/// Resuelve el reporte de pacientes usando exclusivamente el cliente HTTP de pacientes.
/// </summary>
public sealed class GetReportePacientesQueryHandler : IRequestHandler<GetReportePacientesQuery, PagedDto<List<PacienteReportDto>>>
{
    private readonly IPacientesApiClient _pacientesClient;

    /// <summary>Inicializa el handler con el cliente HTTP tipado.</summary>
    public GetReportePacientesQueryHandler(IPacientesApiClient pacientesClient)
    {
        _pacientesClient = pacientesClient;
    }

    /// <inheritdoc />
    public async Task<PagedDto<List<PacienteReportDto>>> Handle(GetReportePacientesQuery request, CancellationToken cancellationToken)
    {
        var paged = await _pacientesClient.GetPagedAsync(new PacienteFilterRequest
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            Documento = request.Documento,
            Nombre = request.Nombre,
            FechaDesde = request.FechaDesde,
            FechaHasta = request.FechaHasta
        }, cancellationToken);

        var data = paged?.Data?.Select(paciente => paciente.ToReportDto()).ToList() ?? new List<PacienteReportDto>();

        return new PagedDto<List<PacienteReportDto>>(
            paged?.TotalRecords ?? data.Count,
            paged?.CurrentPage ?? request.PageNumber,
            paged?.PageSize ?? request.PageSize,
            data);
    }
}
