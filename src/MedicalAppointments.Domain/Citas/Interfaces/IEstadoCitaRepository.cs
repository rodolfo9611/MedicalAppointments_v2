using MedicalAppointments.Domain.Citas.Entities;
using Nuget.Persistence.Abstractions;

namespace MedicalAppointments.Domain.Citas.Interfaces;

public interface IEstadoCitaRepository : IRepository<EstadoCita, int>
{
}