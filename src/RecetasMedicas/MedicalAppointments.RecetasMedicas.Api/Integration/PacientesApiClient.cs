using System.Net;
using System.Net.Http.Json;
using MedicalAppointments.RecetasMedicas.Domain.Integration;
using MedicalAppointments.RecetasMedicas.Domain.Interfaces;

namespace MedicalAppointments.RecetasMedicas.Api.Integration;

/// <summary>Cliente HTTP tipado para datos y eventos del historial de pacientes.</summary>
public sealed class PacientesApiClient : IPacientesApiClient
{
    private readonly HttpClient _httpClient;

    /// <summary>Inicializa el cliente con un HttpClient configurado para Pacientes.</summary>
    /// <param name="httpClient">Cliente HTTP tipado.</param>
    public PacientesApiClient(HttpClient httpClient) => _httpClient = httpClient;

    /// <inheritdoc />
    public async Task<PacienteSnapshotDto?> GetPacienteAsync(
        long pacienteId,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync($"api/Pacientes/{pacienteId}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PacienteSnapshotDto>(cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<int>> GetSecuenciasAtencionAsync(
        long citaId,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync(
            $"api/Pacientes/historial/cita/{citaId}/atenciones",
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return Array.Empty<int>();

        response.EnsureSuccessStatusCode();
        var secuencias = await response.Content.ReadFromJsonAsync<List<int>>(cancellationToken: cancellationToken);
        return secuencias ?? new List<int>();
    }

    /// <inheritdoc />
    public async Task RegistrarEventoAsync(
        EventoHistorialRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PostAsJsonAsync(
            $"api/Pacientes/{request.PacienteID}/historial/eventos",
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();
    }
}
