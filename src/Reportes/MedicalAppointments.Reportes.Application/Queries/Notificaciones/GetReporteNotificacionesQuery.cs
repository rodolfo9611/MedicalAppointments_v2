using MediatR;
using MedicalAppointments.Reportes.Application.Common;
using MedicalAppointments.Reportes.Application.DTOs;
using Nuget.Persistence.Common;

namespace MedicalAppointments.Reportes.Application.Queries.Notificaciones;

/// <summary>
/// Consulta paginada de notificaciones con filtros de tipo, estado, canal y rango de fechas.
/// </summary>
public sealed class GetReporteNotificacionesQuery : PagedQueryBase, IRequest<PagedDto<List<NotificacionReportDto>>>
{
    /// <summary>Tipo de notificación.</summary>
    public string? Tipo { get; set; }

    /// <summary>Estado de la notificación.</summary>
    public string? Estado { get; set; }

    /// <summary>Canal de la notificación.</summary>
    public string? Canal { get; set; }

    /// <summary>Fecha inicial del rango.</summary>
    public DateTime? FechaDesde { get; set; }

    /// <summary>Fecha final del rango.</summary>
    public DateTime? FechaHasta { get; set; }
}
