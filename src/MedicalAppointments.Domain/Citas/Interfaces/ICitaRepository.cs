using MedicalAppointments.Domain.Citas.Entities;
using Nuget.Persistence.Abstractions;

namespace MedicalAppointments.Domain.Citas.Interfaces;

public interface ICitaRepository : IRepository<Cita, long>
{
}