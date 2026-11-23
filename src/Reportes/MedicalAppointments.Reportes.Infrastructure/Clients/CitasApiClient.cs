using System.Globalization;
using System.Text.Json;
using MedicalAppointments.Reportes.Domain.Common;
using MedicalAppointments.Reportes.Domain.DTOs;
using MedicalAppointments.Reportes.Domain.Interfaces;
using MedicalAppointments.Reportes.Domain.Requests;
using Nuget.Persistence.Common;

namespace MedicalAppointments.Reportes.Infrastructure.Clients;

/// <summary>
/// Cliente HTTP tipado de solo lectura para <c>/api/Citas</c>.
/// Los conteos usan páginas de tamaño 1 y <c>TotalRecords</c>; el resto de filtros se aplican en memoria.
/// </summary>
public sealed class CitasApiClient : ApiClientBase, ICitasApiClient
{
    private const int FetchPageSize = 500;
    private const int MaxPages = 10000;
    private readonly HttpClient _httpClient;

    /// <summary>Inicializa el cliente con el <c>HttpClient</c> provisto por <c>IHttpClientFactory</c>.</summary>
    public CitasApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <inheritdoc />
    public async Task<PagedDto<List<CitaReadDto>>?> GetPagedAsync(CitaFilterRequest request, CancellationToken cancellationToken = default)
    {
        var citas = (await GetAllAsync(request, cancellationToken)).AsEnumerable();

        if (request.EstadoCitaId.HasValue)
        {
            var estado = request.EstadoCitaId.Value;
            citas = citas.Where(c => c.EstadoCitaID == estado || (int)c.Estado == estado);
        }

        if (request.PacienteId.HasValue)
        {
            var pacienteId = request.PacienteId.Value;
            citas = citas.Where(c => c.PacienteID == pacienteId);
        }

        if (request.FechaDesde.HasValue)
        {
            var desde = request.FechaDesde.Value;
            citas = citas.Where(c => c.FechaHoraInicio >= desde);
        }

        if (request.FechaHasta.HasValue)
        {
            var hasta = request.FechaHasta.Value;
            citas = citas.Where(c => c.FechaHoraInicio <= hasta);
        }

        return Paginate(citas.ToList(), request.PageNumber, request.PageSize);
    }

    /// <inheritdoc />
    public async Task<int> CountDistinctAsync(EstadoCitaRead estado, CancellationToken cancellationToken = default)
    {
        var valorEstado = (int)estado;
        var url = "/api/Citas/paged" + BuildQuery(
            ("pageNumber", "1"),
            ("pageSize", "1"),
            ("filter", $"EstadoCitaID == {valorEstado.ToString(CultureInfo.InvariantCulture)}"));

        try
        {
            var response = await _httpClient.GetAsync(url, cancellationToken);
            var paged = await ReadPagedAsync<CitaReadDto>(response, cancellationToken);

            // El endpoint de citas devuelve una fila por cita, por lo que TotalRecords
            // equivale al número de CitaID distintos del estado solicitado.
            if (paged is { TotalRecords: > 0 })
            {
                return paged.TotalRecords;
            }
        }
        catch (HttpRequestException)
        {
            // Si el filtro dinámico no está disponible se fuerza el conteo en memoria.
        }
        catch (JsonException)
        {
            // Respuesta no paginada: se aplica el mismo fallback.
        }

        var todas = await GetAllAsync(new CitaFilterRequest(), cancellationToken);
        return todas
            .Where(c => c.EstadoCitaID == valorEstado || (int)c.Estado == valorEstado)
            .Select(c => c.CitaID)
            .Distinct()
            .Count();
    }

    /// <inheritdoc />
    public async Task<int> CountDistinctTodayAsync(DateTime fechaDesde, DateTime fechaHasta, CancellationToken cancellationToken = default)
    {
        var citas = await GetAllAsync(new CitaFilterRequest(), cancellationToken);

        return citas
            .Where(c => c.FechaHoraInicio >= fechaDesde && c.FechaHoraInicio < fechaHasta)
            .Select(c => c.CitaID)
            .Distinct()
            .Count();
    }

    private async Task<List<CitaReadDto>> GetAllAsync(CitaFilterRequest request, CancellationToken cancellationToken)
    {
        var resultado = new List<CitaReadDto>();

        for (var page = 1; page <= MaxPages; page++)
        {
            var url = "/api/Citas/paged" + BuildQuery(
                ("pageNumber", page.ToString(CultureInfo.InvariantCulture)),
                ("pageSize", FetchPageSize.ToString(CultureInfo.InvariantCulture)),
                ("filter", request.Filter),
                ("orderBy", request.OrderBy));

            var response = await _httpClient.GetAsync(url, cancellationToken);
            var paged = await ReadPagedAsync<CitaReadDto>(response, cancellationToken);
            var data = paged?.Data ?? new List<CitaReadDto>();

            resultado.AddRange(data);

            if (data.Count == 0 || paged?.TotalRecords > 0 && resultado.Count >= paged.TotalRecords)
            {
                break;
            }

            if (paged?.TotalPage > 0 && page >= paged.TotalPage)
            {
                break;
            }
        }

        return resultado;
    }
}
