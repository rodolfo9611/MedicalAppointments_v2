using MediatR;
using MicroservicioPacientes.Application.DTOs;
using MicroservicioPacientes.Application.Mappings;
using MicroservicioPacientes.Domain;
using Nuget.Persistence.Common;

namespace MicroservicioPacientes.Application.Queries.GetPacienteOneBy;

/// <summary>Consulta un único paciente mediante una expresión de filtro dinámica.</summary>
public sealed record GetPacienteOneByQuery(string Filter) : IRequest<GetPacienteDto?>;

/// <summary>Manejador de <see cref="GetPacienteOneByQuery"/>.</summary>
public sealed class GetPacienteOneByQueryHandler : IRequestHandler<GetPacienteOneByQuery, GetPacienteDto?>
{
    private readonly IPacienteRepository _pacienteRepository;

    public GetPacienteOneByQueryHandler(IPacienteRepository pacienteRepository)
    {
        _pacienteRepository = pacienteRepository;
    }

    public async Task<GetPacienteDto?> Handle(GetPacienteOneByQuery request, CancellationToken cancellationToken)
    {
        var paciente = await _pacienteRepository.GetOneByAsync(
            Filter.FromStringExpression<Paciente>(request.Filter),
            asNoTracking: true,
            cancellationToken,
            p => p.Historiales);

        return paciente is null ? null : PacienteMappings.ToDto(paciente);
    }
}
