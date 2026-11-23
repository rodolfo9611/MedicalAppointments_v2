using MedicalAppointments.Notifications.Domain.Enums;
using Nuget.Persistence.Common;

namespace MedicalAppointments.Notifications.Application.DTOs;

/// <summary>
/// Parámetros de paginación y filtrado del listado de notificaciones.
/// </summary>
public class NotificacionFiltroDto : RequestParametersGets
{
    public string? Estado { get; set; }
    public string? Canal { get; set; }

    /// <summary>Tipo de notificación solicitado (query string: <c>tipo</c>).</summary>
    public TipoNotificacionEnum? Tipo { get; set; }

    /// <summary>Alias opcional para clientes que envían <c>tipoNotificacion</c>.</summary>
    public TipoNotificacionEnum? TipoNotificacion { get; set; }

    public DateTime? FechaDesde { get; set; }
    public DateTime? FechaHasta { get; set; }
}
