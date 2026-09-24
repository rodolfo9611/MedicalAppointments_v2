using MicroservicioPacientes.Domain;
using Nuget.Persistence.Abstractions;

namespace MicroservicioPacientes.Application;

public interface IPacienteRepository : IRepository<Paciente, int>
{
}