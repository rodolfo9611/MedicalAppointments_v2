using MediatR;
using MedicalAppointments.RecetasMedicas.Application.Common;
using MedicalAppointments.RecetasMedicas.Application.DTOs;
using MedicalAppointments.RecetasMedicas.Domain.Interfaces;
using Nuget.Persistence.Abstractions;

namespace MedicalAppointments.RecetasMedicas.Application.Queries.GetRecetasVigentes;

/// <summary>Listado paginado de recetas vigentes (no anuladas).</summary>
public sealed record GetRecetasVigentesQuery(int PageNumber = 1, int PageSize = 10)
    : IRequest<PagedResult<RecetaMedicaDto>>;

public sealed class GetRecetasVigentesQueryHandler
    : IRequestHandler<GetRecetasVigentesQuery, PagedResult<RecetaMedicaDto>>
{
    private readonly IRecetaMedicaRepository _repository;

    public GetRecetasVigentesQueryHandler(IRecetaMedicaRepository repository) => _repository = repository;

    public async Task<PagedResult<RecetaMedicaDto>> Handle(
        GetRecetasVigentesQuery request,
        CancellationToken cancellationToken)
    {
        var pagina = await _repository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            filter: r => !r.EsAnulada,
            orderByExpression: q => q.OrderByDescending(r => r.FechaEmision),
            cancellationToken: cancellationToken);

        return pagina.ToPagedDto();
    }
}
