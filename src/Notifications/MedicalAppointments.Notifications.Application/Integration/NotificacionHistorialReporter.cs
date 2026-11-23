using System.Collections.Concurrent;
using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MedicalAppointments.Notifications.Application.Integration;

/// <summary>
/// Registra en Pacientes los envíos de notificaciones como eventos idempotentes del historial clínico.
/// Los fallos de integración nunca interrumpen el envío ni al Worker.
/// </summary>
public interface INotificacionHistorialReporter
{
    Task ReportarEnvioAsync(NotificacionEnviadaEvent evento, CancellationToken cancellationToken = default);
}

/// <summary>
/// Contrato enviado a POST /api/Pacientes/{id}/historial/eventos.
/// </summary>
public sealed class EventoHistorialNotificacionDto
{
    public long PacienteID { get; set; }
    public string TipoEvento { get; set; } = "NotificacionEnviada";
    public string Descripcion { get; set; } = string.Empty;
    public string EventKey { get; set; } = string.Empty;
    public DateTime? FechaEvento { get; set; }
    public long? CitaID { get; set; }
    public long? NotificacionID { get; set; }
}

public sealed class NotificacionHistorialReporter : INotificacionHistorialReporter
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<NotificacionHistorialReporter> _logger;
    private readonly ConcurrentDictionary<string, byte> _eventosReportados = new();

    public NotificacionHistorialReporter(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<NotificacionHistorialReporter> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task ReportarEnvioAsync(
        NotificacionEnviadaEvent evento,
        CancellationToken cancellationToken = default)
    {
        if (!string.Equals(evento.Estado, "Enviado", StringComparison.OrdinalIgnoreCase))
            return;

        var eventKey = $"NOTIFICACION:{evento.NotificacionID}:ENVIADA";

        // La primera notificación de este proceso gana; el endpoint de Pacientes
        // garantiza la idempotencia frente a reinicios mediante el mismo EventKey.
        if (!_eventosReportados.TryAdd(eventKey, 0))
            return;

        try
        {
            using var client = _httpClientFactory.CreateClient("PacientesClient");
            if (client.BaseAddress is null)
            {
                var url = _configuration["ApiUrls:Pacientes"] ?? "http://localhost:5015";
                client.BaseAddress = new Uri(url);
            }
            client.Timeout = TimeSpan.FromSeconds(10);

            var payload = new EventoHistorialNotificacionDto
            {
                PacienteID = evento.PacienteID,
                TipoEvento = "NotificacionEnviada",
                EventKey = eventKey,
                NotificacionID = evento.NotificacionID,
                CitaID = evento.CitaID,
                Descripcion = ConstruirDescripcion(evento),
                FechaEvento = evento.FechaEnvio ?? DateTime.UtcNow
            };

            var response = await client.PostAsJsonAsync(
                $"api/Pacientes/{evento.PacienteID}/historial/eventos",
                payload,
                cancellationToken);

            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                _logger.LogInformation(
                    "ℹ️ [Notificaciones] Evento {EventKey} ya registrado en el historial clínico.", eventKey);
                return;
            }

            if (!response.IsSuccessStatusCode)
            {
                _eventosReportados.TryRemove(eventKey, out _);
                _logger.LogWarning(
                    "⚠️ [Notificaciones] Pacientes rechazó el evento {EventKey} ({StatusCode}).",
                    eventKey,
                    response.StatusCode);
                return;
            }

            _logger.LogInformation(
                "📚 [Notificaciones] Evento {EventKey} consolidado en el historial clínico.", eventKey);
        }
        catch (Exception ex)
        {
            _eventosReportados.TryRemove(eventKey, out _);
            _logger.LogWarning(
                ex,
                "⚠️ [Notificaciones] No se pudo registrar el evento {EventKey} en Pacientes. La notificación no se vio afectada.",
                eventKey);
        }
    }

    private static string ConstruirDescripcion(NotificacionEnviadaEvent evento)
    {
        var mensaje = evento.Mensaje ?? string.Empty;
        if (mensaje.Length > 120)
            mensaje = mensaje[..120] + "...";

        return $"Notificación {evento.TipoNotificacion} enviada vía {evento.Canal}. {mensaje}".Trim();
    }
}
