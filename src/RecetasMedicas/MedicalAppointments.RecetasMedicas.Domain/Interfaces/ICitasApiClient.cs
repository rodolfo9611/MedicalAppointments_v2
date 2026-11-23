using MedicalAppointments.RecetasMedicas.Domain.Integration;

namespace MedicalAppointments.RecetasMedicas.Domain.Interfaces;

/// <summary>Consulta de citas al microservicio de Citas (http://localhost:5000).</summary>
public interface ICitasApiClient
{
    Task<CitaSnapshotDto?> GetCitaAsync(long citaId, CancellationToken cancellationToken = default);
}
