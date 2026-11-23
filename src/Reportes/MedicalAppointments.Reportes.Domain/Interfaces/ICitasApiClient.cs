using MedicalAppointments.Reportes.Domain.Common;
using MedicalAppointments.Reportes.Domain.DTOs;
using MedicalAppointments.Reportes.Domain.Requests;
using Nuget.Persistence.Common;

namespace MedicalAppointments.Reportes.Domain.Interfaces;

/// <summary>
/// Cliente de solo lectura para el microservicio de citas.
/// </summary>
public interface ICitasApiClient
{
    /// <summary>Obtiene citas paginadas aplicando los filtros del reporte.</summary>
    Task<PagedDto<List<CitaReadDto>>?> GetPagedAsync(CitaFilterRequest request, CancellationToken cancellationToken = default);

    /// <summary>Cuenta identificadores de cita distintos para un estado usando una consulta paginada.</summary>
    Task<int> CountDistinctAsync(EstadoCitaRead estado, CancellationToken cancellationToken = default);

    /// <summary>Cuenta identificadores de cita distintos cuyo inicio cae en el rango indicado.</summary>
    Task<int> CountDistinctTodayAsync(DateTime fechaDesde, DateTime fechaHasta, CancellationToken cancellationToken = default);
}
