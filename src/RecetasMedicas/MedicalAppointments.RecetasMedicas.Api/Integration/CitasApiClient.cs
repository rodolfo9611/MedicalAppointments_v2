using System.Net;
using System.Net.Http.Json;
using MedicalAppointments.RecetasMedicas.Domain.Integration;
using MedicalAppointments.RecetasMedicas.Domain.Interfaces;

namespace MedicalAppointments.RecetasMedicas.Api.Integration;

/// <summary>Cliente HTTP tipado para GET /api/Citas/{id}.</summary>
public sealed class CitasApiClient : ICitasApiClient
{
    private readonly HttpClient _httpClient;

    /// <summary>Inicializa el cliente con un HttpClient configurado para la API de Citas.</summary>
    /// <param name="httpClient">Cliente HTTP tipado.</param>
    public CitasApiClient(HttpClient httpClient) => _httpClient = httpClient;

    /// <inheritdoc />
    public async Task<CitaSnapshotDto?> GetCitaAsync(
        long citaId,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync($"api/Citas/{citaId}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CitaSnapshotDto>(cancellationToken: cancellationToken);
    }
}
