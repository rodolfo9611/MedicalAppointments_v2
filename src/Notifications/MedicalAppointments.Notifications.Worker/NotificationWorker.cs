using System.Net.Http.Json;
using System.Text.Json;
using MedicalAppointments.Notifications.Domain.Entities;
using MedicalAppointments.Notifications.Domain.Interfaces;
using MedicalAppointments.Notifications.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MedicalAppointments.Notifications.Worker;

public class NotificationWorker : BackgroundService
{
    private readonly ILogger<NotificationWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public NotificationWorker(
        ILogger<NotificationWorker> logger,
        IServiceScopeFactory scopeFactory,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("🔔 [NotificationWorker] Servicio de Recordatorios y Notificaciones iniciado correctamente.");

        // Inicializar base de datos / tablas en el primer ciclo
        using (var initialScope = _scopeFactory.CreateScope())
        {
            var dbContext = initialScope.ServiceProvider.GetRequiredService<NotificationsDbContext>();
            try
            {
                await dbContext.Database.EnsureCreatedAsync(stoppingToken);
                _logger.LogInformation("✅ [NotificationWorker] Base de datos y tablas de Notificaciones verificadas.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning("⚠️ [NotificationWorker] No se pudo verificar la BD automáticamente: {Msg}", ex.Message);
            }
        }

        var pollingIntervalSeconds = _configuration.GetValue<int>("WorkerSettings:PollingIntervalSeconds", 20);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("⏱️ [NotificationWorker] Ejecutando escaneo periódico a las: {Time}", DateTimeOffset.Now);

                await ProcesarCitasAsync(stoppingToken);
                await DespacharRecordatoriosPendientesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [NotificationWorker] Error inesperado en el ciclo del Worker.");
            }

            await Task.Delay(TimeSpan.FromSeconds(pollingIntervalSeconds), stoppingToken);
        }

        _logger.LogInformation("🛑 [NotificationWorker] Servicio detenido.");
    }

    private async Task ProcesarCitasAsync(CancellationToken stoppingToken)
    {
        var citasApiUrl = _configuration["CitasApiUrl"] ?? "http://localhost:5000";
        var client = _httpClientFactory.CreateClient("CitasClient");
        client.BaseAddress = new Uri(citasApiUrl);
        client.Timeout = TimeSpan.FromSeconds(10);

        List<CitaDto>? citas = null;

        try
        {
            citas = await client.GetFromJsonAsync<List<CitaDto>>("api/Citas", cancellationToken: stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning("⚠️ [NotificationWorker] No se pudo consultar la API de Citas ({Url}): {Error}. Se reintentará en el siguiente ciclo.", citasApiUrl, ex.Message);
            return;
        }

        if (citas == null || citas.Count == 0)
        {
            _logger.LogInformation("ℹ️ [NotificationWorker] No se encontraron citas para procesar.");
            return;
        }

        using var scope = _scopeFactory.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<INotificacionRepository>();

        foreach (var cita in citas)
        {
            if (stoppingToken.IsCancellationRequested) break;

            var existentes = await repo.GetByCitaIdAsync(cita.CitaID);
            if (existentes.Any())
            {
                continue; // Ya tiene recordatorio generado
            }

            // Seleccionar canal y plantilla según regla de negocio
            var canal = (cita.CitaID % 3) switch
            {
                0 => "WhatsApp",
                1 => "SMS",
                _ => "Email"
            };

            var destinatario = canal switch
            {
                "Email" => $"paciente{cita.PacienteID}@correo-clinica.com",
                _ => $"+503 7{cita.PacienteID:D3}-{(cita.CitaID * 111) % 10000:D4}"
            };

            var nuevaNotificacion = new Notificacion
            {
                CitaID = cita.CitaID,
                PacienteID = cita.PacienteID,
                PacienteNombre = $"Paciente #{cita.PacienteID}",
                Canal = canal,
                Destinatario = destinatario,
                FechaProgramadaCita = cita.FechaHoraInicio,
                FechaRegistro = DateTime.UtcNow,
                Estado = "Pendiente",
                Intentos = 0,
                Mensaje = $"Estimado paciente #{cita.PacienteID}, le recordamos su cita médica el {cita.FechaHoraInicio:dd/MM/yyyy HH:mm} en la clínica. Motivo: {cita.Motivo ?? "Consulta de Control"}."
            };

            await repo.AddAsync(nuevaNotificacion);
            _logger.LogInformation("📝 [NotificationWorker] Creada notificación pendiente para Cita #{CitaID} (Canal: {Canal})", cita.CitaID, canal);
        }
    }

    private async Task DespacharRecordatoriosPendientesAsync(CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<INotificacionRepository>();

        var pendientes = (await repo.GetPendientesAsync()).ToList();

        if (!pendientes.Any())
        {
            return;
        }

        _logger.LogInformation("📤 [NotificationWorker] Despachando {Count} recordatorio(s) pendiente(s)...", pendientes.Count);

        foreach (var notif in pendientes)
        {
            if (stoppingToken.IsCancellationRequested) break;

            // Simular envío a través del canal externo (SMS / WhatsApp / Email Gateway)
            await Task.Delay(300, stoppingToken);

            notif.Intentos++;
            notif.FechaEnvio = DateTime.UtcNow;
            notif.Estado = "Enviado";
            notif.DetalleRespuesta = $"Despachado exitosamente a través de Pasarela {notif.Canal}. Código de rastreo: ACK-{Guid.NewGuid().ToString()[..8].ToUpper()}";

            await repo.UpdateAsync(notif);

            _logger.LogInformation("✅ [NotificationWorker] ENVIADO recordatorio Cita #{CitaID} a {Destinatario} ({Canal}).", 
                notif.CitaID, notif.Destinatario, notif.Canal);
        }
    }
}

public class CitaDto
{
    public long CitaID { get; set; }
    public long PacienteID { get; set; }
    public int MedicoID { get; set; }
    public int EstadoCitaID { get; set; }
    public DateTime FechaHoraInicio { get; set; }
    public DateTime? FechaHoraFin { get; set; }
    public string? Motivo { get; set; }
}
