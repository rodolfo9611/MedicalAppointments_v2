using MedicalAppointments.Domain.Citas.Integration;

namespace MedicalAppointments.Domain.Citas.Interfaces;

/// <summary>
/// Puerto de integración con el historial clínico del microservicio de Pacientes.
/// </summary>
public interface IHistorialClinicoClient
{
    Task RegistrarEventoAsync(EventoHistorialRequest request, CancellationToken cancellationToken = default);
    Task<bool> ExisteAtencionAsync(long citaId, CancellationToken cancellationToken = default);
    Task<bool> ExisteAtencionAsync(long citaId, int secuencia, CancellationToken cancellationToken = default);
    Task<int> ObtenerSiguienteSecuenciaAtencionAsync(long citaId, CancellationToken cancellationToken = default);
}
