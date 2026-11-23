using MedicalAppointments.RecetasMedicas.Domain.Entities;
using Nuget.Persistence.Abstractions;

namespace MedicalAppointments.RecetasMedicas.Domain.Interfaces;

/// <summary>
/// Repositorio de recetas: CRUD genérico heredado más consultas específicas.
/// </summary>
public interface IRecetaMedicaRepository : IRepository<RecetaMedica, long>
{
    Task<RecetaMedica?> GetActivaByCitaIdAsync(long citaId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RecetaMedica>> GetByCitaIdAsync(long citaId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RecetaMedica>> GetByPacienteIdAsync(
        long pacienteId,
        bool incluirAnuladas = false,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RecetaMedica>> GetAnuladasAsync(CancellationToken cancellationToken = default);

    /// <summary>Última receta anulada de una cita; se usa como RecetaAnteriorID de una corrección.</summary>
    Task<long?> GetUltimaAnuladaIdByCitaIdAsync(long citaId, CancellationToken cancellationToken = default);

    Task<PagedResult<RecetaMedica>> GetPagedFilteredAsync(
        int pageNumber,
        int pageSize,
        long? pacienteId,
        string? diagnostico,
        DateTime? fechaDesde,
        DateTime? fechaHasta,
        bool incluirAnuladas,
        CancellationToken cancellationToken = default);
}
