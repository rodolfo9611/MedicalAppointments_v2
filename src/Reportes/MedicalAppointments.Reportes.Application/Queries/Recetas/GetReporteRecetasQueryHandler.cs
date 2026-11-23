using MediatR;
using MedicalAppointments.Reportes.Application.DTOs;
using MedicalAppointments.Reportes.Application.Extensions;
using MedicalAppointments.Reportes.Domain.Interfaces;
using MedicalAppointments.Reportes.Domain.Requests;
using Nuget.Persistence.Common;

namespace MedicalAppointments.Reportes.Application.Queries.Recetas;

/// <summary>
/// Resuelve el reporte de recetas usando exclusivamente el cliente HTTP de recetas.
/// </summary>
public sealed class GetReporteRecetasQueryHandler : IRequestHandler<GetReporteRecetasQuery, PagedDto<List<RecetaReportDto>>>
{
    private readonly IRecetasApiClient _recetasClient;

    /// <summary>Inicializa el handler con el cliente HTTP tipado.</summary>
    public GetReporteRecetasQueryHandler(IRecetasApiClient recetasClient)
    {
        _recetasClient = recetasClient;
    }

    /// <inheritdoc />
    public async Task<PagedDto<List<RecetaReportDto>>> Handle(GetReporteRecetasQuery request, CancellationToken cancellationToken)
    {
        var paged = await _recetasClient.GetPagedAsync(new RecetaFilterRequest
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            PacienteId = request.PacienteId,
            Diagnostico = request.Diagnostico,
            FechaDesde = request.FechaDesde,
            FechaHasta = request.FechaHasta,
            IncluirAnuladas = false
        }, cancellationToken);

        var data = paged?.Data?.Select(receta => receta.ToReportDto()).ToList() ?? new List<RecetaReportDto>();

        return new PagedDto<List<RecetaReportDto>>(
            paged?.TotalRecords ?? data.Count,
            paged?.CurrentPage ?? request.PageNumber,
            paged?.PageSize ?? request.PageSize,
            data);
    }
}
