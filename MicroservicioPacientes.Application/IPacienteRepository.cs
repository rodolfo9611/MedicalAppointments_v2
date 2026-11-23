using MicroservicioPacientes.Domain;
using Nuget.Persistence.Abstractions;

namespace MicroservicioPacientes.Application;

/// <summary>
/// Repositorio de pacientes. Extiende el repositorio genérico compartido con
/// operaciones específicas del historial clínico, sin crear repositorios nuevos.
/// </summary>
public interface IPacienteRepository : IRepository<Paciente, int>
{
    /// <summary>Obtiene una página de eventos del historial de un paciente.</summary>
    Task<PagedResult<HistorialMedico>> GetHistorialPagedAsync(
        int pacienteId,
        string? tipoEvento,
        DateTime? fechaDesde,
        DateTime? fechaHasta,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    /// <summary>Busca un evento por su clave idempotente.</summary>
    Task<HistorialMedico?> GetEventoByKeyAsync(string eventKey, CancellationToken cancellationToken = default);

    /// <summary>Agrega un evento al historial clínico.</summary>
    Task AddEventoAsync(HistorialMedico evento, CancellationToken cancellationToken = default);

    /// <summary>
    /// Devuelve, ordenadas, las secuencias de atención registradas para una cita.
    /// </summary>
    Task<int[]> GetSecuenciasAtencionAsync(long citaId, CancellationToken cancellationToken = default);
}
