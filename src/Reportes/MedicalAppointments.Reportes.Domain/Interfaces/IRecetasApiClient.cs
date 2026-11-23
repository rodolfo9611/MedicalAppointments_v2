using MedicalAppointments.Reportes.Domain.DTOs;
using MedicalAppointments.Reportes.Domain.Requests;
using Nuget.Persistence.Common;

namespace MedicalAppointments.Reportes.Domain.Interfaces;

/// <summary>
/// Cliente de solo lectura para el microservicio de recetas médicas.
/// </summary>
public interface IRecetasApiClient
{
    /// <summary>Obtiene recetas paginadas; siempre solicita <c>incluirAnuladas=false</c>.</summary>
    Task<PagedDto<List<RecetaReadDto>>?> GetPagedAsync(RecetaFilterRequest request, CancellationToken cancellationToken = default);

    /// <summary>Obtiene el total de recetas vigentes leyendo <c>TotalRecords</c>.</summary>
    Task<int> CountAsync(RecetaFilterRequest request, CancellationToken cancellationToken = default);
}
