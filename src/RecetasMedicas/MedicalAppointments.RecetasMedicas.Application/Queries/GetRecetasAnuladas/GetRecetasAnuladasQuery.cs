using MediatR;
using MedicalAppointments.RecetasMedicas.Application.Common;
using MedicalAppointments.RecetasMedicas.Application.DTOs;
using MedicalAppointments.RecetasMedicas.Domain.Interfaces;

namespace MedicalAppointments.RecetasMedicas.Application.Queries.GetRecetasAnuladas;

/// <summary>Listado de recetas anuladas lógicamente.</summary>
public sealed record GetRecetasAnuladasQuery : IRequest<IReadOnlyList<RecetaMedicaDto>>;

public sealed class GetRecetasAnuladasQueryHandler
    : IRequestHandler<GetRecetasAnuladasQuery, IReadOnlyList<RecetaMedicaDto>>
{
    private readonly IRecetaMedicaRepository _repository;

    public GetRecetasAnuladasQueryHandler(IRecetaMedicaRepository repository) => _repository = repository;

    public async Task<IReadOnlyList<RecetaMedicaDto>> Handle(
        GetRecetasAnuladasQuery request,
        CancellationToken cancellationToken)
    {
        var recetas = await _repository.GetAnuladasAsync(cancellationToken);
        return recetas.Select(r => r.ToDto()).ToList();
    }
}
