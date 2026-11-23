using MediatR;
using MicroservicioPacientes.Application.DTOs;
using MicroservicioPacientes.Application.Mappings;

namespace MicroservicioPacientes.Application.Queries.GetPacienteById;

/// <summary>Consulta un paciente por su identificador.</summary>
public sealed record GetPacienteByIdQuery(int Id) : IRequest<GetPacienteDto?>;

/// <summary>Manejador de <see cref="GetPacienteByIdQuery"/>.</summary>
public sealed class GetPacienteByIdQueryHandler : IRequestHandler<GetPacienteByIdQuery, GetPacienteDto?>
{
    private readonly IPacienteRepository _pacienteRepository;

    public GetPacienteByIdQueryHandler(IPacienteRepository pacienteRepository)
    {
        _pacienteRepository = pacienteRepository;
    }

    public async Task<GetPacienteDto?> Handle(GetPacienteByIdQuery request, CancellationToken cancellationToken)
    {
        var paciente = await _pacienteRepository.GetOneByAsync(
            p => p.Id == request.Id,
            asNoTracking: true,
            cancellationToken,
            p => p.Historiales);

        return paciente is null ? null : PacienteMappings.ToDto(paciente);
    }
}
