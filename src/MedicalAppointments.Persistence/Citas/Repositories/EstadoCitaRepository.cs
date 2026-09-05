using MedicalAppointments.Domain.Citas.Entities;
using MedicalAppointments.Domain.Citas.Interfaces;
using MedicalAppointments.Persistence.Citas.Context;
using MedicalAppointments.Persistence.Common.Repositories;

namespace MedicalAppointments.Persistence.Citas.Repositories;

public class EstadoCitaRepository : GenericRepository<EstadoCita, int>, IEstadoCitaRepository
{
    public EstadoCitaRepository(CitasDbContext context) : base(context)
    {
    }
}