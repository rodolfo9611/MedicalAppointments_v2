using MediatR;
using MicroservicioPacientes.Application.DTOs;
using MicroservicioPacientes.Application.Mappings;

namespace MicroservicioPacientes.Application.Queries.GetPacientes;

/// <summary>Consulta el listado completo de pacientes.</summary>
public sealed record GetPacientesQuery : IRequest<IReadOnlyList<GetPacienteDto>>;

/// <summary>Manejador de <see cref="GetPacientesQuery"/>.</summary>
public sealed class GetPacientesQueryHandler : IRequestHandler<GetPacientesQuery, IReadOnlyList<GetPacienteDto>>
{
    private readonly IPacienteRepository _pacienteRepository;

    public GetPacientesQueryHandler(IPacienteRepository pacienteRepository)
    {
        _pacienteRepository = pacienteRepository;
    }

    public async Task<IReadOnlyList<GetPacienteDto>> Handle(GetPacientesQuery request, CancellationToken cancellationToken)
    {
        var pacientes = await _pacienteRepository.GetAllAsync(asNoTracking: true, p => p.Historiales);
        return pacientes.Select(PacienteMappings.ToDto).ToList();
    }
}
