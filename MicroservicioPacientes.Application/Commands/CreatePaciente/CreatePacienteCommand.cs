using MediatR;
using MicroservicioPacientes.Application.DTOs;
using MicroservicioPacientes.Application.Exceptions;
using MicroservicioPacientes.Application.Mappings;
using MicroservicioPacientes.Domain;

namespace MicroservicioPacientes.Application.Commands.CreatePaciente;

/// <summary>Crea un nuevo paciente.</summary>
public sealed record CreatePacienteCommand(CreatePacienteDto Paciente) : IRequest<GetPacienteDto>;

/// <summary>Manejador de <see cref="CreatePacienteCommand"/>.</summary>
public sealed class CreatePacienteCommandHandler : IRequestHandler<CreatePacienteCommand, GetPacienteDto>
{
    private readonly IPacienteRepository _pacienteRepository;

    public CreatePacienteCommandHandler(IPacienteRepository pacienteRepository)
    {
        _pacienteRepository = pacienteRepository;
    }

    public async Task<GetPacienteDto> Handle(CreatePacienteCommand request, CancellationToken cancellationToken)
    {
        var codigo = request.Paciente.CodigoPaciente.Trim();

        var existente = await _pacienteRepository.GetOneByAsync(
            p => p.CodigoPaciente == codigo,
            asNoTracking: true,
            cancellationToken);

        if (existente is not null)
            throw new DuplicateCodigoPacienteException(codigo);

        var paciente = new Paciente();
        PacienteMappings.Apply(paciente, request.Paciente);

        await _pacienteRepository.AddAsync(paciente);
        return PacienteMappings.ToDto(paciente);
    }
}
