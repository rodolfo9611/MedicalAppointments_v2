namespace MedicalAppointments.Reportes.Application.DTOs;

/// <summary>
/// Indicadores agregados del dashboard. Todas las cifras son conteos de registros de negocio.
/// </summary>
public sealed class DashboardDto
{
    /// <summary>Total de pacientes.</summary>
    public int TotalPacientes { get; set; }

    /// <summary>Citas en estado Agendada.</summary>
    public int CitasAgendadas { get; set; }

    /// <summary>Citas en estado Citada.</summary>
    public int CitasCitadas { get; set; }

    /// <summary>Citas en estado Pendiente de resultados.</summary>
    public int CitasPendienteResultados { get; set; }

    /// <summary>Citas en estado Concluida.</summary>
    public int CitasConcluidas { get; set; }

    /// <summary>Citas en estado Cancelada.</summary>
    public int CitasCanceladas { get; set; }

    /// <summary>Recetas emitidas (anuladas excluidas).</summary>
    public int RecetasEmitidas { get; set; }

    /// <summary>Notificaciones generadas informadas por el servicio de notificaciones.</summary>
    public int NotificacionesGeneradas { get; set; }

    /// <summary>Citas con fecha de inicio de hoy.</summary>
    public int CitasHoy { get; set; }

    /// <summary>Recetas emitidas hoy.</summary>
    public int RecetasHoy { get; set; }

    /// <summary>Notificaciones generadas hoy.</summary>
    public int NotificacionesHoy { get; set; }
}
