using MediatR;
using MedicalAppointments.Reportes.Application.DTOs;
using MedicalAppointments.Reportes.Application.Extensions;
using MedicalAppointments.Reportes.Domain.Interfaces;
using MedicalAppointments.Reportes.Domain.Requests;
using Nuget.Persistence.Common;

namespace MedicalAppointments.Reportes.Application.Queries.Notificaciones;

/// <summary>
/// Resuelve el reporte de notificaciones usando exclusivamente el cliente HTTP de notificaciones.
/// </summary>
public sealed class GetReporteNotificacionesQueryHandler : IRequestHandler<GetReporteNotificacionesQuery, PagedDto<List<NotificacionReportDto>>>
{
    private readonly INotificacionesApiClient _notificacionesClient;

    /// <summary>Inicializa el handler con el cliente HTTP tipado.</summary>
    public GetReporteNotificacionesQueryHandler(INotificacionesApiClient notificacionesClient)
    {
        _notificacionesClient = notificacionesClient;
    }

    /// <inheritdoc />
    public async Task<PagedDto<List<NotificacionReportDto>>> Handle(GetReporteNotificacionesQuery request, CancellationToken cancellationToken)
    {
        var paged = await _notificacionesClient.GetPagedAsync(new NotificacionFilterRequest
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            Tipo = request.Tipo,
            Estado = request.Estado,
            Canal = request.Canal,
            FechaDesde = request.FechaDesde,
            FechaHasta = request.FechaHasta
        }, cancellationToken);

        var data = paged?.Data?.Select(notificacion => notificacion.ToReportDto()).ToList() ?? new List<NotificacionReportDto>();

        return new PagedDto<List<NotificacionReportDto>>(
            paged?.TotalRecords ?? data.Count,
            paged?.CurrentPage ?? request.PageNumber,
            paged?.PageSize ?? request.PageSize,
            data);
    }
}
