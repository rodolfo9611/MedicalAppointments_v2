using System.Globalization;
using System.Text.Json;
using MedicalAppointments.Reportes.Domain.DTOs;
using MedicalAppointments.Reportes.Domain.Interfaces;
using MedicalAppointments.Reportes.Domain.Requests;
using Nuget.Persistence.Common;

namespace MedicalAppointments.Reportes.Infrastructure.Clients;

/// <summary>
/// Cliente HTTP tipado de solo lectura para <c>/api/Notificaciones</c>.
/// Las estadísticas se consumen por HTTP sin replicar la lógica del servicio de notificaciones.
/// </summary>
public sealed class NotificacionesApiClient : ApiClientBase, INotificacionesApiClient
{
    private readonly HttpClient _httpClient;

    /// <summary>Inicializa el cliente con el <c>HttpClient</c> provisto por <c>IHttpClientFactory</c>.</summary>
    public NotificacionesApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <inheritdoc />
    public async Task<PagedDto<List<NotificacionReadDto>>?> GetPagedAsync(NotificacionFilterRequest request, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(request);
        var response = await _httpClient.GetAsync(url, cancellationToken);
        return await ReadPagedAsync<NotificacionReadDto>(response, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<int> CountAsync(NotificacionFilterRequest request, CancellationToken cancellationToken = default)
    {
        var countRequest = new NotificacionFilterRequest
        {
            PageNumber = 1,
            PageSize = 1,
            Tipo = request.Tipo,
            Estado = request.Estado,
            Canal = request.Canal,
            FechaDesde = request.FechaDesde,
            FechaHasta = request.FechaHasta
        };

        var paged = await GetPagedAsync(countRequest, cancellationToken);
        return paged?.TotalRecords ?? 0;
    }

    /// <inheritdoc />
    public async Task<NotificacionStatsDto?> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("/api/Notificaciones/stats", cancellationToken);
        response.EnsureSuccessStatusCode();

        if (response.Content.Headers.ContentLength == 0)
        {
            return null;
        }

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
        var root = document.RootElement;

        if (root.ValueKind == JsonValueKind.Object &&
            TryGetPropertyIgnoreCase(root, "data", out var data) &&
            data.ValueKind == JsonValueKind.Object)
        {
            root = data;
        }

        return new NotificacionStatsDto
        {
            TotalNotificaciones = ReadInt(root, "totalNotificaciones"),
            Enviadas = ReadInt(root, "enviadas"),
            Pendientes = ReadInt(root, "pendientes"),
            Fallidas = ReadInt(root, "fallidas"),
            TasaEfectividadPorcentaje = ReadDouble(root, "tasaEfectividadPorcentaje")
        };
    }

    private string BuildUrl(NotificacionFilterRequest request)
    {
        return "/api/Notificaciones/paged" + BuildQuery(
            ("pageNumber", request.PageNumber.ToString(CultureInfo.InvariantCulture)),
            ("pageSize", request.PageSize.ToString(CultureInfo.InvariantCulture)),
            ("tipo", request.Tipo),
            ("estado", request.Estado),
            ("canal", request.Canal),
            ("fechaDesde", request.FechaDesde.HasValue ? FormatDate(request.FechaDesde.Value) : null),
            ("fechaHasta", request.FechaHasta.HasValue ? FormatDate(request.FechaHasta.Value) : null));
    }

    private static int ReadInt(JsonElement root, string name) =>
        TryGetPropertyIgnoreCase(root, name, out var value) && value.TryGetInt32(out var number) ? number : 0;

    private static double ReadDouble(JsonElement root, string name) =>
        TryGetPropertyIgnoreCase(root, name, out var value) && value.TryGetDouble(out var number) ? number : 0d;
}
