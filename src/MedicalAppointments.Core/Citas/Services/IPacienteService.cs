using MedicalAppointments.Core.Citas.DTOs;

namespace MedicalAppointments.Core.Citas.Services;

/// <summary>
/// Contrato en términos de negocio del microservicio de Pacientes.
/// El handler depende de esta interfaz, nunca de IRest: así la lógica de negocio
/// se puede probar sin levantar el otro servicio.
/// </summary>
public interface IPacienteService
{
    Task<GetPacienteExternoDto?> ObtenerPacienteAsync(long pacienteId, CancellationToken cancellationToken = default);
}
