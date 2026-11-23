using Nuget.Persistence.Common;

namespace MedicalAppointments.Reportes.Domain.Requests;

/// <summary>
/// Filtros enviados al cliente HTTP del microservicio de notificaciones.
/// </summary>
public sealed class NotificacionFilterRequest : RequestParametersGets
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
