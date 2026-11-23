using MediatR;

namespace MicroservicioPacientes.Application.Commands.DeletePaciente;

/// <summary>Elimina un paciente existente.</summary>
public sealed record DeletePacienteCommand(int Id) : IRequest<bool>;

/// <summary>Manejador de <see cref="DeletePacienteCommand"/>.</summary>
public sealed class DeletePacienteCommandHandler : IRequestHandler<DeletePacienteCommand, bool>
{
    private readonly IPacienteRepository _pacienteRepository;

    public DeletePacienteCommandHandler(IPacienteRepository pacienteRepository)
    {
        _pacienteRepository = pacienteRepository;
    }

    public async Task<bool> Handle(DeletePacienteCommand request, CancellationToken cancellationToken)
    {
        var paciente = await _pacienteRepository.GetByIdAsync(request.Id);
        if (paciente is null)
            return false;

        await _pacienteRepository.DeleteAsync(paciente);
        return true;
    }
}
