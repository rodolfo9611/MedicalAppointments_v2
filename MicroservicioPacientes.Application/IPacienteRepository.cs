using MicroservicioPacientes.Domain;

namespace MicroservicioPacientes.Application;

public interface IPacienteRepository
{
    Task<IEnumerable<Paciente>> ObtenerTodosAsync();
    Task<Paciente?> ObtenerPorIdAsync(int id);
    Task AgregarAsync(Paciente paciente);
}