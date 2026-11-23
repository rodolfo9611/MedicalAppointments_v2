using MediatR;
using MedicalAppointments.Notifications.Application.DTOs;
using Nuget.Persistence.Common;

namespace MedicalAppointments.Notifications.Application.Queries.GetNotificacionById;

/// <summary>
/// Obtiene una notificación por su identificador.
/// </summary>
public sealed record GetNotificacionByIdQuery(long Id)
    : IRequest<HttpResponse<GetNotificacionDto>>;
