using MedicalAppointments.Domain.Citas.Entities;
using MedicalAppointments.Domain.Citas.Interfaces;
using MedicalAppointments.Persistence.Citas.Context;
using Nuget.Persistence.Implementations;

namespace MedicalAppointments.Persistence.Citas.Repositories;

public class EstadoCitaRepository : Repository<EstadoCita, int>, IEstadoCitaRepository
{
    public EstadoCitaRepository(CitasDbContext context) : base(context)
    {
    }
}
