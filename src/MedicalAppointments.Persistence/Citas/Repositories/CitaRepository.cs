using MedicalAppointments.Domain.Citas.Entities;
using MedicalAppointments.Domain.Citas.Interfaces;
using MedicalAppointments.Persistence.Citas.Context;
using Microsoft.EntityFrameworkCore;

namespace MedicalAppointments.Persistence.Citas.Repositories;

public class CitaRepository : ICitaRepository
{
    private readonly CitasDbContext _context;

    public CitaRepository(CitasDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Cita>> GetAllAsync()
    {
        return await _context.Citas.ToListAsync();
    }

    public async Task<Cita?> GetByIdAsync(long id)
    {
        return await _context.Citas.FindAsync(id);
    }
}
