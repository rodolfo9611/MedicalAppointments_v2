using MedicalAppointments.RecetasMedicas.Domain.Integration;

namespace MedicalAppointments.RecetasMedicas.Domain.Interfaces;

/// <summary>Integración con el microservicio de Pacientes (http://localhost:5015).</summary>
public interface IPacientesApiClient
{
    Task<PacienteSnapshotDto?> GetPacienteAsync(long pacienteId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<int>> GetSecuenciasAtencionAsync(long citaId, CancellationToken cancellationToken = default);

    Task RegistrarEventoAsync(EventoHistorialRequest request, CancellationToken cancellationToken = default);
}
