using MediatR;
using MedicalAppointments.RecetasMedicas.Application.Common;
using MedicalAppointments.RecetasMedicas.Application.Queries.GetRecetaById;
using MedicalAppointments.RecetasMedicas.Domain.Interfaces;

namespace MedicalAppointments.RecetasMedicas.Application.Queries.GetRecetaImprimible;

/// <summary>Devuelve el HTML imprimible de una receta, o null si no existe.</summary>
public sealed record GetRecetaImprimibleQuery(long RecetaMedicaID) : IRequest<string?>;

public sealed class GetRecetaImprimibleQueryHandler : IRequestHandler<GetRecetaImprimibleQuery, string?>
{
    private readonly IMediator _mediator;

    public GetRecetaImprimibleQueryHandler(IMediator mediator) => _mediator = mediator;

    public async Task<string?> Handle(GetRecetaImprimibleQuery request, CancellationToken cancellationToken)
    {
        var receta = await _mediator.Send(new GetRecetaByIdQuery(request.RecetaMedicaID), cancellationToken);
        return receta is null ? null : RecetaHtmlRenderer.Render(receta);
    }
}
