using System.Linq.Expressions;
using MedicalAppointments.RecetasMedicas.Domain.Entities;
using MedicalAppointments.RecetasMedicas.Domain.Interfaces;
using MedicalAppointments.RecetasMedicas.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Nuget.Persistence.Abstractions;
using Nuget.Persistence.Implementations;

namespace MedicalAppointments.RecetasMedicas.Persistence.Repositories;

public class RecetaMedicaRepository : Repository<RecetaMedica, long>, IRecetaMedicaRepository
{
    private readonly RecetasDbContext _context;

    public RecetaMedicaRepository(RecetasDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<RecetaMedica?> GetActivaByCitaIdAsync(
        long citaId,
        CancellationToken cancellationToken = default)
        => await _context.RecetasMedicas
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.CitaID == citaId && !r.EsAnulada, cancellationToken);

    public async Task<IReadOnlyList<RecetaMedica>> GetByCitaIdAsync(
        long citaId,
        CancellationToken cancellationToken = default)
        => await _context.RecetasMedicas
            .AsNoTracking()
            .Where(r => r.CitaID == citaId)
            .OrderByDescending(r => r.FechaEmision)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<RecetaMedica>> GetByPacienteIdAsync(
        long pacienteId,
        bool incluirAnuladas = false,
        CancellationToken cancellationToken = default)
        => await _context.RecetasMedicas
            .AsNoTracking()
            .Where(r => r.PacienteID == pacienteId && (incluirAnuladas || !r.EsAnulada))
            .OrderByDescending(r => r.FechaEmision)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<RecetaMedica>> GetAnuladasAsync(CancellationToken cancellationToken = default)
        => await _context.RecetasMedicas
            .AsNoTracking()
            .Where(r => r.EsAnulada)
            .OrderByDescending(r => r.FechaAnulacion)
            .ToListAsync(cancellationToken);

    public async Task<long?> GetUltimaAnuladaIdByCitaIdAsync(
        long citaId,
        CancellationToken cancellationToken = default)
        => await _context.RecetasMedicas
            .AsNoTracking()
            .Where(r => r.CitaID == citaId && r.EsAnulada)
            .OrderByDescending(r => r.RecetaMedicaID)
            .Select(r => (long?)r.RecetaMedicaID)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<PagedResult<RecetaMedica>> GetPagedFilteredAsync(
        int pageNumber,
        int pageSize,
        long? pacienteId,
        string? diagnostico,
        DateTime? fechaDesde,
        DateTime? fechaHasta,
        bool incluirAnuladas,
        CancellationToken cancellationToken = default)
    {
        Expression<Func<RecetaMedica, bool>> filtro = r =>
            (incluirAnuladas || !r.EsAnulada) &&
            (!pacienteId.HasValue || r.PacienteID == pacienteId.Value) &&
            (string.IsNullOrWhiteSpace(diagnostico) || r.Diagnostico.Contains(diagnostico)) &&
            (!fechaDesde.HasValue || r.FechaEmision >= fechaDesde.Value) &&
            (!fechaHasta.HasValue || r.FechaEmision <= fechaHasta.Value);

        return await GetPagedAsync(
            pageNumber,
            pageSize,
            filter: filtro,
            orderByExpression: q => q.OrderByDescending(r => r.FechaEmision),
            cancellationToken: cancellationToken);
    }
}
