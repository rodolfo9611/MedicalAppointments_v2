using MedicalAppointments.Notifications.Domain.Entities;
using MedicalAppointments.Notifications.Domain.Interfaces;
using MedicalAppointments.Notifications.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Nuget.Persistence.Implementations;

namespace MedicalAppointments.Notifications.Persistence.Repositories;

public class NotificacionRepository : Repository<Notificacion, long>, INotificacionRepository
{
    private readonly NotificationsDbContext _notificationsContext;

    public NotificacionRepository(NotificationsDbContext context)
        : base(context)
    {
        _notificationsContext = context;
    }

    public async Task<IEnumerable<Notificacion>> GetByCitaIdAsync(long citaId)
    {
        return await _notificationsContext.Notificaciones
            .AsNoTracking()
            .Where(n => n.CitaID == citaId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Notificacion>> GetPendientesAsync()
    {
        return await _notificationsContext.Notificaciones
            .Where(n => n.Estado == "Pendiente")
            .OrderBy(n => n.FechaProgramadaCita)
            .ToListAsync();
    }
}
