using MedicalAppointments.Reportes.Domain.DTOs;
using MedicalAppointments.Reportes.Domain.Requests;
using Nuget.Persistence.Common;

namespace MedicalAppointments.Reportes.Domain.Interfaces;

/// <summary>
/// Cliente de solo lectura para el microservicio de notificaciones.
/// </summary>
public interface INotificacionesApiClient
{
    /// <summary>Obtiene notificaciones paginadas aplicando los filtros del reporte.</summary>
    Task<PagedDto<List<NotificacionReadDto>>?> GetPagedAsync(NotificacionFilterRequest request, CancellationToken cancellationToken = default);

    /// <summary>Obtiene el total de notificaciones leyendo <c>TotalRecords</c>.</summary>
    Task<int> CountAsync(NotificacionFilterRequest request, CancellationToken cancellationToken = default);

    /// <summary>Consulta por HTTP las estadísticas publicadas por el servicio de notificaciones.</summary>
    Task<NotificacionStatsDto?> GetStatsAsync(CancellationToken cancellationToken = default);
}
