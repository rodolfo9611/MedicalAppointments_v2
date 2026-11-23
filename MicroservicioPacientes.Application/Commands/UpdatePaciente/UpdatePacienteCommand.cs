using MediatR;
using MicroservicioPacientes.Application.DTOs;
using MicroservicioPacientes.Application.Exceptions;
using MicroservicioPacientes.Application.Mappings;

namespace MicroservicioPacientes.Application.Commands.UpdatePaciente;

/// <summary>Actualiza los datos maestros de un paciente existente.</summary>
public sealed record UpdatePacienteCommand(int Id, UpdatePacienteDto Paciente) : IRequest<GetPacienteDto>;

/// <summary>Manejador de <see cref="UpdatePacienteCommand"/>.</summary>
public sealed class UpdatePacienteCommandHandler : IRequestHandler<UpdatePacienteCommand, GetPacienteDto>
{
    private readonly IPacienteRepository _pacienteRepository;

    public UpdatePacienteCommandHandler(IPacienteRepository pacienteRepository)
    {
        _pacienteRepository = pacienteRepository;
    }

    public async Task<GetPacienteDto> Handle(UpdatePacienteCommand request, CancellationToken cancellationToken)
    {
        var paciente = await _pacienteRepository.GetByIdAsync(request.Id);
        if (paciente is null)
            throw new PacienteNotFoundException(request.Id);

        var codigo = request.Paciente.CodigoPaciente.Trim();
        var duplicado = await _pacienteRepository.GetOneByAsync(
            p => p.CodigoPaciente == codigo && p.Id != request.Id,
            asNoTracking: true,
            cancellationToken);

        if (duplicado is not null)
            throw new DuplicateCodigoPacienteException(codigo);

        PacienteMappings.Apply(paciente, request.Paciente);
        await _pacienteRepository.UpdateAsync(paciente);

        return PacienteMappings.ToDto(paciente);
    }
}
