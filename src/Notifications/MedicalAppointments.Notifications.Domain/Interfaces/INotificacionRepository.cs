using MedicalAppointments.Notifications.Domain.Entities;
using Nuget.Persistence.Abstractions;

namespace MedicalAppointments.Notifications.Domain.Interfaces;

public interface INotificacionRepository : IRepository<Notificacion, long>
{
    Task<IEnumerable<Notificacion>> GetByCitaIdAsync(long citaId);
    Task<IEnumerable<Notificacion>> GetPendientesAsync();
}
