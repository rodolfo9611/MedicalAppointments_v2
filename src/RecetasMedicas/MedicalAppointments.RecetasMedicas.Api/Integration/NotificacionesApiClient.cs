using System.Net.Http.Json;
using MedicalAppointments.RecetasMedicas.Domain.Integration;
using MedicalAppointments.RecetasMedicas.Domain.Interfaces;

namespace MedicalAppointments.RecetasMedicas.Api.Integration;

/// <summary>Cliente HTTP tipado para POST /api/Notificaciones.</summary>
public sealed class NotificacionesApiClient : INotificacionesApiClient
{
    private readonly HttpClient _httpClient;

    /// <summary>Inicializa el cliente con un HttpClient configurado para Notificaciones.</summary>
    /// <param name="httpClient">Cliente HTTP tipado.</param>
    public NotificacionesApiClient(HttpClient httpClient) => _httpClient = httpClient;

    /// <inheritdoc />
    public async Task RegistrarNotificacionAsync(
        CreateNotificacionDto request,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PostAsJsonAsync("api/Notificaciones", request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
