using MediatR;
using MedicalAppointments.Reportes.Application.DTOs;
using MedicalAppointments.Reportes.Domain.Common;
using MedicalAppointments.Reportes.Domain.DTOs;
using MedicalAppointments.Reportes.Domain.Interfaces;
using MedicalAppointments.Reportes.Domain.Requests;
using Microsoft.Extensions.Logging;

namespace MedicalAppointments.Reportes.Application.Queries.Dashboard;

/// <summary>
/// Compone el dashboard exclusivamente con datos obtenidos por HTTP de los otros microservicios.
/// </summary>
public sealed class GetDashboardQueryHandler : IRequestHandler<GetDashboardQuery, DashboardDto>
{
    private readonly IPacientesApiClient _pacientesClient;
    private readonly ICitasApiClient _citasClient;
    private readonly IRecetasApiClient _recetasClient;
    private readonly INotificacionesApiClient _notificacionesClient;
    private readonly ILogger<GetDashboardQueryHandler> _logger;

    /// <summary>Inicializa el handler con los clientes HTTP tipados.</summary>
    public GetDashboardQueryHandler(
        IPacientesApiClient pacientesClient,
        ICitasApiClient citasClient,
        IRecetasApiClient recetasClient,
        INotificacionesApiClient notificacionesClient,
        ILogger<GetDashboardQueryHandler> logger)
    {
        _pacientesClient = pacientesClient;
        _citasClient = citasClient;
        _recetasClient = recetasClient;
        _notificacionesClient = notificacionesClient;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<DashboardDto> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
    {
        var hoy = DateTime.Today;
        var finHoyExclusivo = hoy.AddDays(1);
        var finHoyInclusivo = finHoyExclusivo.AddSeconds(-1);

        var totalPacientes = await SafeAsync(
            () => _pacientesClient.CountAsync(cancellationToken),
            nameof(DashboardDto.TotalPacientes), 0, cancellationToken);

        var citasAgendadas = await SafeAsync(
            () => _citasClient.CountDistinctAsync(EstadoCitaRead.Agendada, cancellationToken),
            nameof(DashboardDto.CitasAgendadas), 0, cancellationToken);

        var citasCitadas = await SafeAsync(
            () => _citasClient.CountDistinctAsync(EstadoCitaRead.Citada, cancellationToken),
            nameof(DashboardDto.CitasCitadas), 0, cancellationToken);

        var citasPendienteResultados = await SafeAsync(
            () => _citasClient.CountDistinctAsync(EstadoCitaRead.PendienteResultados, cancellationToken),
            nameof(DashboardDto.CitasPendienteResultados), 0, cancellationToken);

        var citasConcluidas = await SafeAsync(
            () => _citasClient.CountDistinctAsync(EstadoCitaRead.Concluida, cancellationToken),
            nameof(DashboardDto.CitasConcluidas), 0, cancellationToken);

        var citasCanceladas = await SafeAsync(
            () => _citasClient.CountDistinctAsync(EstadoCitaRead.Cancelada, cancellationToken),
            nameof(DashboardDto.CitasCanceladas), 0, cancellationToken);

        var citasHoy = await SafeAsync(
            () => _citasClient.CountDistinctTodayAsync(hoy, finHoyExclusivo, cancellationToken),
            nameof(DashboardDto.CitasHoy), 0, cancellationToken);

        var recetasEmitidas = await SafeAsync(
            () => _recetasClient.CountAsync(new RecetaFilterRequest { IncluirAnuladas = false }, cancellationToken),
            nameof(DashboardDto.RecetasEmitidas), 0, cancellationToken);

        var recetasHoy = await SafeAsync(
            () => _recetasClient.CountAsync(new RecetaFilterRequest
            {
                FechaDesde = hoy,
                FechaHasta = finHoyInclusivo,
                IncluirAnuladas = false
            }, cancellationToken),
            nameof(DashboardDto.RecetasHoy), 0, cancellationToken);

        var stats = await SafeAsync<NotificacionStatsDto?>(
            () => _notificacionesClient.GetStatsAsync(cancellationToken),
            "Notificaciones.Stats", null, cancellationToken);

        var notificacionesGeneradas = stats?.TotalNotificaciones ?? await SafeAsync(
            () => _notificacionesClient.CountAsync(new NotificacionFilterRequest(), cancellationToken),
            nameof(DashboardDto.NotificacionesGeneradas), 0, cancellationToken);

        var notificacionesHoy = await SafeAsync(
            () => _notificacionesClient.CountAsync(new NotificacionFilterRequest
            {
                FechaDesde = hoy,
                FechaHasta = finHoyInclusivo
            }, cancellationToken),
            nameof(DashboardDto.NotificacionesHoy), 0, cancellationToken);

        return new DashboardDto
        {
            TotalPacientes = totalPacientes,
            CitasAgendadas = citasAgendadas,
            CitasCitadas = citasCitadas,
            CitasPendienteResultados = citasPendienteResultados,
            CitasConcluidas = citasConcluidas,
            CitasCanceladas = citasCanceladas,
            RecetasEmitidas = recetasEmitidas,
            NotificacionesGeneradas = notificacionesGeneradas,
            CitasHoy = citasHoy,
            RecetasHoy = recetasHoy,
            NotificacionesHoy = notificacionesHoy
        };
    }

    private async Task<T> SafeAsync<T>(Func<Task<T>> action, string metrica, T fallback, CancellationToken cancellationToken)
    {
        try
        {
            return await action();
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "No fue posible obtener la métrica {Metrica} del dashboard desde los servicios remotos.", metrica);
            return fallback;
        }
    }
}
