using System.Net.Http.Json;
using MedicalAppointments.Domain.Citas.Integration;
using MedicalAppointments.Domain.Citas.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MedicalAppointments.Infrastructure.Integration;

/// <summary>
/// Adaptador HTTP hacia el historial clínico del microservicio de Pacientes.
/// </summary>
public class HistorialClinicoClient : IHistorialClinicoClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<HistorialClinicoClient> _logger;

    public HistorialClinicoClient(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<HistorialClinicoClient> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task RegistrarEventoAsync(EventoHistorialRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            using var client = CrearClient();
            var url = $"api/Pacientes/{request.PacienteID}/historial/eventos";
            var response = await client.PostAsJsonAsync(url, request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            // El evento es auditable, pero no debe impedir la operación clínica principal.
            _logger.LogWarning(ex, "No se pudo registrar el evento {EventKey} en el historial clínico.", request.EventKey);
        }
    }

    public async Task<bool> ExisteAtencionAsync(long citaId, CancellationToken cancellationToken = default)
    {
        var secuencias = await ObtenerSecuenciasAtencionAsync(citaId, cancellationToken);
        return secuencias.Count > 0;
    }

    public async Task<bool> ExisteAtencionAsync(long citaId, int secuencia, CancellationToken cancellationToken = default)
    {
        var secuencias = await ObtenerSecuenciasAtencionAsync(citaId, cancellationToken);
        return secuencias.Contains(secuencia);
    }

    public async Task<int> ObtenerSiguienteSecuenciaAtencionAsync(long citaId, CancellationToken cancellationToken = default)
    {
        var secuencias = await ObtenerSecuenciasAtencionAsync(citaId, cancellationToken);
        return secuencias.Count == 0 ? 1 : secuencias.Max() + 1;
    }

    private async Task<List<int>> ObtenerSecuenciasAtencionAsync(long citaId, CancellationToken cancellationToken)
    {
        using var client = CrearClient();
        var result = await client.GetFromJsonAsync<List<int>>(
            $"api/Pacientes/historial/cita/{citaId}/atenciones", cancellationToken);
        return result ?? new List<int>();
    }

    private HttpClient CrearClient()
    {
        var client = _httpClientFactory.CreateClient("PacientesClient");
        if (client.BaseAddress is null)
        {
            var url = _configuration["ApiUrls:Pacientes"] ?? "http://localhost:5015";
            client.BaseAddress = new Uri(url);
        }
        client.Timeout = TimeSpan.FromSeconds(15);
        return client;
    }
}
