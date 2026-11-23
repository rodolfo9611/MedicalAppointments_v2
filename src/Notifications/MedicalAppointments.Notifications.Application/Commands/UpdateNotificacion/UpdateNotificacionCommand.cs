using MediatR;
using MedicalAppointments.Notifications.Application.DTOs;
using Nuget.Persistence.Common;

namespace MedicalAppointments.Notifications.Application.Commands.UpdateNotificacion;

/// <summary>
/// Actualiza una notificación existente.
/// </summary>
public sealed record UpdateNotificacionCommand(long Id, UpdateNotificacionDto Notificacion)
    : IRequest<HttpResponse<GetNotificacionDto>>;
