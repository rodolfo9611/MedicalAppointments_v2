using MediatR;
using MedicalAppointments.RecetasMedicas.Application.Common;
using MedicalAppointments.RecetasMedicas.Application.DTOs;
using MedicalAppointments.RecetasMedicas.Domain.Interfaces;

namespace MedicalAppointments.RecetasMedicas.Application.Queries.GetRecetaById;

/// <summary>Detalle de una receta, incluidas las anuladas.</summary>
public sealed record GetRecetaByIdQuery(long RecetaMedicaID) : IRequest<RecetaMedicaDto?>;

public sealed class GetRecetaByIdQueryHandler : IRequestHandler<GetRecetaByIdQuery, RecetaMedicaDto?>
{
    private readonly IRecetaMedicaRepository _repository;

    public GetRecetaByIdQueryHandler(IRecetaMedicaRepository repository) => _repository = repository;

    public async Task<RecetaMedicaDto?> Handle(GetRecetaByIdQuery request, CancellationToken cancellationToken)
    {
        var receta = await _repository.GetByIdAsync(request.RecetaMedicaID);
        return receta?.ToDto();
    }
}
