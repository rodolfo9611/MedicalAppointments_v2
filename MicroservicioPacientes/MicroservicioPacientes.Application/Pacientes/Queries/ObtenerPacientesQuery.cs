using MediatR;
using MicroservicioPacientes.Domain;

namespace MicroservicioPacientes.Application.Pacientes.Queries;

// Request
public record ObtenerPacientesQuery() : IRequest<IEnumerable<Paciente>>;

// Handler
public class ObtenerPacientesQueryHandler : IRequestHandler<ObtenerPacientesQuery, IEnumerable<Paciente>>
{
    private readonly IPacienteRepository _repository;

    public ObtenerPacientesQueryHandler(IPacienteRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Paciente>> Handle(ObtenerPacientesQuery request, CancellationToken cancellationToken)
    {
        return await _repository.ObtenerTodosAsync();
    }
}