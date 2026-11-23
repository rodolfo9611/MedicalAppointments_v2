using MediatR;
using MicroservicioPacientes.Application.DTOs;
using MicroservicioPacientes.Application.Exceptions;
using MicroservicioPacientes.Application.Mappings;
using MicroservicioPacientes.Domain;

namespace MicroservicioPacientes.Application.Commands.CreatePacientesRange;

/// <summary>Crea un rango de pacientes en una sola operación.</summary>
public sealed record CreatePacientesRangeCommand(
    IReadOnlyList<CreatePacienteDto> Pacientes) : IRequest<int>;

/// <summary>Manejador de <see cref="CreatePacientesRangeCommand"/>.</summary>
public sealed class CreatePacientesRangeCommandHandler : IRequestHandler<CreatePacientesRangeCommand, int>
{
    private readonly IPacienteRepository _pacienteRepository;

    public CreatePacientesRangeCommandHandler(IPacienteRepository pacienteRepository)
    {
        _pacienteRepository = pacienteRepository;
    }

    public async Task<int> Handle(CreatePacientesRangeCommand request, CancellationToken cancellationToken)
    {
        var codigos = request.Pacientes
            .Select(p => p.CodigoPaciente.Trim())
            .ToList();

        var codigoDuplicado = codigos
            .GroupBy(c => c, StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault(g => g.Count() > 1)?.Key;

        if (codigoDuplicado is not null)
            throw new DuplicateCodigoPacienteException(codigoDuplicado);

        foreach (var codigo in codigos)
        {
            var existente = await _pacienteRepository.GetOneByAsync(
                p => p.CodigoPaciente == codigo,
                asNoTracking: true,
                cancellationToken);

            if (existente is not null)
                throw new DuplicateCodigoPacienteException(codigo);
        }

        var pacientes = new List<Paciente>(request.Pacientes.Count);
        foreach (var dto in request.Pacientes)
        {
            var paciente = new Paciente();
            PacienteMappings.Apply(paciente, dto);
            pacientes.Add(paciente);
        }

        await _pacienteRepository.AddRangeAsync(pacientes, cancellationToken);
        return pacientes.Count;
    }
}
