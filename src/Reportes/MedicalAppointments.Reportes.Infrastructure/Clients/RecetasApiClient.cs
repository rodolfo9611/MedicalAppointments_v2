using System.Globalization;
using MedicalAppointments.Reportes.Domain.DTOs;
using MedicalAppointments.Reportes.Domain.Interfaces;
using MedicalAppointments.Reportes.Domain.Requests;
using Nuget.Persistence.Common;

namespace MedicalAppointments.Reportes.Infrastructure.Clients;

/// <summary>
/// Cliente HTTP tipado de solo lectura para <c>/api/RecetasMedicas</c>.
/// Toda consulta envía <c>incluirAnuladas=false</c> para excluir recetas anuladas.
/// </summary>
public sealed class RecetasApiClient : ApiClientBase, IRecetasApiClient
{
    private readonly HttpClient _httpClient;

    /// <summary>Inicializa el cliente con el <c>HttpClient</c> provisto por <c>IHttpClientFactory</c>.</summary>
    public RecetasApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <inheritdoc />
    public async Task<PagedDto<List<RecetaReadDto>>?> GetPagedAsync(RecetaFilterRequest request, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(request);
        var response = await _httpClient.GetAsync(url, cancellationToken);
        return await ReadPagedAsync<RecetaReadDto>(response, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<int> CountAsync(RecetaFilterRequest request, CancellationToken cancellationToken = default)
    {
        var countRequest = new RecetaFilterRequest
        {
            PageNumber = 1,
            PageSize = 1,
            PacienteId = request.PacienteId,
            Diagnostico = request.Diagnostico,
            FechaDesde = request.FechaDesde,
            FechaHasta = request.FechaHasta,
            IncluirAnuladas = false
        };

        var paged = await GetPagedAsync(countRequest, cancellationToken);
        return paged?.TotalRecords ?? 0;
    }

    private string BuildUrl(RecetaFilterRequest request)
    {
        return "/api/RecetasMedicas/paged" + BuildQuery(
            ("pageNumber", request.PageNumber.ToString(CultureInfo.InvariantCulture)),
            ("pageSize", request.PageSize.ToString(CultureInfo.InvariantCulture)),
            ("pacienteId", request.PacienteId?.ToString(CultureInfo.InvariantCulture)),
            ("diagnostico", request.Diagnostico),
            ("fechaDesde", request.FechaDesde.HasValue ? FormatDate(request.FechaDesde.Value) : null),
            ("fechaHasta", request.FechaHasta.HasValue ? FormatDate(request.FechaHasta.Value) : null),
            ("incluirAnuladas", "false"));
    }
}
