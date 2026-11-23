using MedicalAppointments.RecetasMedicas.Domain.Integration;

namespace MedicalAppointments.RecetasMedicas.Domain.Interfaces;

/// <summary>Registro de notificaciones en el microservicio de Notificaciones (http://localhost:5100).</summary>
public interface INotificacionesApiClient
{
    Task RegistrarNotificacionAsync(CreateNotificacionDto request, CancellationToken cancellationToken = default);
}
