using Nuget.Persistence.Common;

namespace MedicalAppointments.Notifications.Application.DTOs;

/// <summary>
/// Respuesta paginada de notificaciones basada en el contrato genérico de Nuget.Persistence.
/// </summary>
public class PagedNotificacionDto : PagedDto<List<GetNotificacionDto>>
{
}
