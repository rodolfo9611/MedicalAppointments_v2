using MicroservicioPacientes.Application;
using MicroservicioPacientes.Domain;
using Nuget.Persistence.Implementations;

namespace MicroservicioPacientes.Infrastructure;

public class PacienteRepository : Repository<Paciente, int>, IPacienteRepository
{
    public PacienteRepository(ApplicationDbContext context) : base(context)
    {
    }
}