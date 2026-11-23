using MedicalAppointments.Reportes.Domain.DTOs;
using MedicalAppointments.Reportes.Domain.Requests;
using Nuget.Persistence.Common;

namespace MedicalAppointments.Reportes.Domain.Interfaces;

/// <summary>
/// Cliente de solo lectura para el microservicio de pacientes.
/// </summary>
public interface IPacientesApiClient
{
    /// <summary>Obtiene pacientes paginados aplicando los filtros del reporte.</summary>
    Task<PagedDto<List<PacienteReadDto>>?> GetPagedAsync(PacienteFilterRequest request, CancellationToken cancellationToken = default);

    /// <summary>Obtiene el total de pacientes leyendo <c>TotalRecords</c> de una consulta paginada.</summary>
    Task<int> CountAsync(CancellationToken cancellationToken = default);
}
