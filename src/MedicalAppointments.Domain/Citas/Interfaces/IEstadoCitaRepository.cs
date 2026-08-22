using MedicalAppointments.Domain.Citas.Entities;

namespace MedicalAppointments.Domain.Citas.Interfaces;

public interface IEstadoCitaRepository
{
    Task<IEnumerable<EstadoCita>> GetAllAsync();
    Task<EstadoCita?> GetByIdAsync(int id);
}
