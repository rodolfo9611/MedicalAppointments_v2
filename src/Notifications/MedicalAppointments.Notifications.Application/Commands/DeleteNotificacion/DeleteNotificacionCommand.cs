using MediatR;
using Nuget.Persistence.Common;

namespace MedicalAppointments.Notifications.Application.Commands.DeleteNotificacion;

/// <summary>
/// Elimina una notificación por identificador.
/// </summary>
public sealed record DeleteNotificacionCommand(long Id) : IRequest<HttpResponse<bool>>;
