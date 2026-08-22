using MedicalAppointments.Domain.Citas.Entities;
using MedicalAppointments.Domain.Citas.Interfaces;
using MedicalAppointments.Persistence.Citas.Context;
using Microsoft.EntityFrameworkCore;

namespace MedicalAppointments.Persistence.Citas.Repositories;

public class EstadoCitaRepository : IEstadoCitaRepository
{
    private readonly CitasDbContext _context;

    public EstadoCitaRepository(CitasDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<EstadoCita>> GetAllAsync()
    {
        return await _context.EstadosCita.ToListAsync();
    }

    public async Task<EstadoCita?> GetByIdAsync(int id)
    {
        return await _context.EstadosCita.FindAsync(id);
    }
}
