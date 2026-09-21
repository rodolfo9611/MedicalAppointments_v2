using MedicalAppointments.Domain.Citas.Entities;
using MedicalAppointments.Domain.Citas.Interfaces;
using MedicalAppointments.Persistence.Citas.Context;
using Nuget.Persistence.Implementations;

namespace MedicalAppointments.Persistence.Citas.Repositories;

public class CitaRepository : Repository<Cita, long>, ICitaRepository
{
    public CitaRepository(CitasDbContext context) : base(context)
    {
    }
}
