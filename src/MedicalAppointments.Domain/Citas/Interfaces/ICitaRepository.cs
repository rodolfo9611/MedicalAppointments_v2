using MedicalAppointments.Domain.Citas.Entities;

namespace MedicalAppointments.Domain.Citas.Interfaces;

public interface ICitaRepository
{
    Task<IEnumerable<Cita>> GetAllAsync();
    Task<Cita?> GetByIdAsync(long id);
}
