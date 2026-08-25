using MediatR;
using MicroservicioPacientes.Domain;

namespace MicroservicioPacientes.Application.Pacientes.Commands;

// Request
public record CrearPacienteCommand(string Nombre, string Apellido, DateTime FechaNacimiento) : IRequest<int>;

// Handler
public class CrearPacienteCommandHandler : IRequestHandler<CrearPacienteCommand, int>
{
    private readonly IPacienteRepository _repository;

    public CrearPacienteCommandHandler(IPacienteRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> Handle(CrearPacienteCommand request, CancellationToken cancellationToken)
    {
        var paciente = new Paciente
        {
            Nombre = request.Nombre,
            Apellido = request.Apellido,
            FechaNacimiento = request.FechaNacimiento
        };

        await _repository.AgregarAsync(paciente);
        return paciente.Id;
    }
}