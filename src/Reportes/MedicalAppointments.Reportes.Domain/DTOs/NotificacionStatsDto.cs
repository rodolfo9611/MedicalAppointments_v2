namespace MedicalAppointments.Reportes.Domain.DTOs;

/// <summary>
/// Estadísticas publicadas por el microservicio de notificaciones. No se recalculan en reportes.
/// </summary>
public sealed class NotificacionStatsDto
{
    /// <summary>Total de notificaciones generadas.</summary>
    public int TotalNotificaciones { get; set; }

    /// <summary>Notificaciones enviadas.</summary>
    public int Enviadas { get; set; }

    /// <summary>Notificaciones pendientes.</summary>
    public int Pendientes { get; set; }

    /// <summary>Notificaciones fallidas.</summary>
    public int Fallidas { get; set; }

    /// <summary>Tasa de efectividad informada por el servicio de notificaciones.</summary>
    public double TasaEfectividadPorcentaje { get; set; }
}
