using MediatR;
using MedicalAppointments.Notifications.Application.DTOs;
using Nuget.Persistence.Common;

namespace MedicalAppointments.Notifications.Application.Queries.GetNotificacionesPaged;

/// <summary>
/// Listado paginado y filtrado por estado, canal, tipo y rango de fechas.
/// </summary>
public sealed class GetNotificacionesPagedQuery : NotificacionFiltroDto,
    IRequest<HttpResponse<PagedNotificacionDto>>
{
}
