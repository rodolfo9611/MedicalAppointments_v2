using MediatR;
using MedicalAppointments.Notifications.Application.DTOs;
using Nuget.Persistence.Common;

namespace MedicalAppointments.Notifications.Application.Commands.CreateNotificacion;

/// <summary>
/// Crea una notificación, incluidas las notificaciones de receta médica.
/// </summary>
public sealed record CreateNotificacionCommand(CreateNotificacionDto Notificacion)
    : IRequest<HttpResponse<GetNotificacionDto>>;
