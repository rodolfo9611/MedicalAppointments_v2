using Microsoft.EntityFrameworkCore;
using MicroservicioPacientes.Application;
using MicroservicioPacientes.Domain;
using Nuget.Persistence.Abstractions;
using Nuget.Persistence.Implementations;

namespace MicroservicioPacientes.Infrastructure;

public class PacienteRepository : Repository<Paciente, int>, IPacienteRepository
{
    private const string TipoEventoAtencionRegistrada = "AtencionRegistrada";

    public PacienteRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<PagedResult<HistorialMedico>> GetHistorialPagedAsync(
        int pacienteId,
        string? tipoEvento,
        DateTime? fechaDesde,
        DateTime? fechaHasta,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1) throw new ArgumentOutOfRangeException(nameof(pageNumber));
        if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize));

        var query = Context.Set<HistorialMedico>()
            .AsNoTracking()
            .Where(h => h.PacienteId == pacienteId);

        if (!string.IsNullOrWhiteSpace(tipoEvento))
            query = query.Where(h => h.TipoEvento == tipoEvento);

        if (fechaDesde.HasValue)
            query = query.Where(h => h.FechaEvento >= fechaDesde.Value);

        if (fechaHasta.HasValue)
            query = query.Where(h => h.FechaEvento <= fechaHasta.Value);

        var totalRecords = await query.CountAsync(cancellationToken);

        var data = await query
            .OrderByDescending(h => h.FechaEvento)
            .ThenByDescending(h => h.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<HistorialMedico>
        {
            Data = data,
            TotalRecords = totalRecords,
            PageSize = pageSize,
            CurrentPage = pageNumber
        };
    }

    public async Task<HistorialMedico?> GetEventoByKeyAsync(
        string eventKey,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<HistorialMedico>()
            .AsNoTracking()
            .FirstOrDefaultAsync(h => h.EventKey == eventKey, cancellationToken);
    }

    public async Task AddEventoAsync(
        HistorialMedico evento,
        CancellationToken cancellationToken = default)
    {
        Context.Set<HistorialMedico>().Add(evento);
        await Context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int[]> GetSecuenciasAtencionAsync(
        long citaId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<HistorialMedico>()
            .AsNoTracking()
            .Where(h => h.CitaID == citaId
                        && h.TipoEvento == TipoEventoAtencionRegistrada
                        && h.SecuenciaAtencion.HasValue)
            .OrderBy(h => h.SecuenciaAtencion)
            .Select(h => h.SecuenciaAtencion!.Value)
            .ToArrayAsync(cancellationToken);
    }
}
