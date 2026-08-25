using Microsoft.EntityFrameworkCore;
using MicroservicioPacientes.Application;
using MicroservicioPacientes.Domain;

namespace MicroservicioPacientes.Infrastructure;

public class PacienteRepository : IPacienteRepository
{
    private readonly ApplicationDbContext _context;

    public PacienteRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Paciente>> ObtenerTodosAsync()
    {
        return await _context.Pacientes
            .Include(p => p.Historiales)
            .ToListAsync();
    }

    public async Task<Paciente?> ObtenerPorIdAsync(int id)
    {
        return await _context.Pacientes
            .Include(p => p.Historiales)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task AgregarAsync(Paciente paciente)
    {
        await _context.Pacientes.AddAsync(paciente);
        await _context.SaveChangesAsync();
    }
}