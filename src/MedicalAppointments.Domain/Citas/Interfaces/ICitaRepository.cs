using MedicalAppointments.Domain.Citas.Entities;
using MedicalAppointments.Domain.Common.Interfaces;

namespace MedicalAppointments.Domain.Citas.Interfaces;

public interface ICitaRepository : IGenericRepository<Cita, long>
{
}