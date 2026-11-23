using System.Globalization;
using MedicalAppointments.Reportes.Domain.DTOs;
using MedicalAppointments.Reportes.Domain.Interfaces;
using MedicalAppointments.Reportes.Domain.Requests;
using Nuget.Persistence.Common;

namespace MedicalAppointments.Reportes.Infrastructure.Clients;

/// <summary>
/// Cliente HTTP tipado de solo lectura para <c>/api/Pacientes</c>.
/// Los filtros de documento, nombre y fecha se aplican de forma explícita en memoria.
/// </summary>
public sealed class PacientesApiClient : ApiClientBase, IPacientesApiClient
{
    private const int FetchPageSize = 500;
    private const int MaxPages = 10000;
    private readonly HttpClient _httpClient;

    /// <summary>Inicializa el cliente con el <c>HttpClient</c> provisto por <c>IHttpClientFactory</c>.</summary>
    public PacientesApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <inheritdoc />
    public async Task<PagedDto<List<PacienteReadDto>>?> GetPagedAsync(PacienteFilterRequest request, CancellationToken cancellationToken = default)
    {
        var pacientes = (await GetAllAsync(request, cancellationToken)).AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request.Documento))
        {
            var documento = request.Documento.Trim();
            pacientes = pacientes.Where(p => p.DocumentoIdentidad.Contains(documento, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Nombre))
        {
            var nombre = request.Nombre.Trim();
            pacientes = pacientes.Where(p =>
                $"{p.Nombre} {p.Apellido}".Contains(nombre, StringComparison.OrdinalIgnoreCase));
        }

        if (request.FechaDesde.HasValue)
        {
            var desde = request.FechaDesde.Value;
            pacientes = pacientes.Where(p => p.FechaNacimiento >= desde);
        }

        if (request.FechaHasta.HasValue)
        {
            var hasta = request.FechaHasta.Value;
            pacientes = pacientes.Where(p => p.FechaNacimiento <= hasta);
        }

        return Paginate(pacientes.ToList(), request.PageNumber, request.PageSize);
    }

    /// <inheritdoc />
    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        var url = "/api/Pacientes/paged" + BuildQuery(
            ("pageNumber", "1"),
            ("pageSize", "1"));

        var response = await _httpClient.GetAsync(url, cancellationToken);
        var paged = await ReadPagedAsync<PacienteReadDto>(response, cancellationToken);

        return paged?.TotalRecords ?? 0;
    }

    private async Task<List<PacienteReadDto>> GetAllAsync(PacienteFilterRequest request, CancellationToken cancellationToken)
    {
        var resultado = new List<PacienteReadDto>();

        for (var page = 1; page <= MaxPages; page++)
        {
            var url = "/api/Pacientes/paged" + BuildQuery(
                ("pageNumber", page.ToString(CultureInfo.InvariantCulture)),
                ("pageSize", FetchPageSize.ToString(CultureInfo.InvariantCulture)),
                ("filter", request.Filter),
                ("orderBy", request.OrderBy));

            var response = await _httpClient.GetAsync(url, cancellationToken);
            var paged = await ReadPagedAsync<PacienteReadDto>(response, cancellationToken);
            var data = paged?.Data ?? new List<PacienteReadDto>();

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
