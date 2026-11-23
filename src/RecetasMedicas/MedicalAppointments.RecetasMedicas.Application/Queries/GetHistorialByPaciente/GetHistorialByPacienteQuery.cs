using MediatR;
using MedicalAppointments.RecetasMedicas.Application.Common;
using MedicalAppointments.RecetasMedicas.Application.DTOs;
using MedicalAppointments.RecetasMedicas.Domain.Interfaces;

namespace MedicalAppointments.RecetasMedicas.Application.Queries.GetHistorialByPaciente;

/// <summary>Historial permanente de recetas de un paciente, incluyendo anuladas.</summary>
public sealed record GetHistorialByPacienteQuery(long PacienteID) : IRequest<IReadOnlyList<RecetaMedicaDto>>;

public sealed class GetHistorialByPacienteQueryHandler
    : IRequestHandler<GetHistorialByPacienteQuery, IReadOnlyList<RecetaMedicaDto>>
{
    private readonly IRecetaMedicaRepository _repository;

    public GetHistorialByPacienteQueryHandler(IRecetaMedicaRepository repository) => _repository = repository;

    public async Task<IReadOnlyList<RecetaMedicaDto>> Handle(
        GetHistorialByPacienteQuery request,
        CancellationToken cancellationToken)
    {
        var recetas = await _repository.GetByPacienteIdAsync(request.PacienteID, true, cancellationToken);
        return recetas.Select(r => r.ToDto()).ToList();
    }
}
