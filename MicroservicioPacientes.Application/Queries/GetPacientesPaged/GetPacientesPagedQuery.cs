using MediatR;
using MicroservicioPacientes.Application.DTOs;
using MicroservicioPacientes.Application.Mappings;
using MicroservicioPacientes.Domain;
using Nuget.Persistence.Abstractions;
using Nuget.Persistence.Common;

namespace MicroservicioPacientes.Application.Queries.GetPacientesPaged;

/// <summary>Consulta pacientes con paginación, filtro y orden dinámicos.</summary>
public sealed record GetPacientesPagedQuery(
    int PageNumber,
    int PageSize,
    string? Filter,
    string? OrderBy) : IRequest<PagedResult<GetPacienteDto>>;

/// <summary>Manejador de <see cref="GetPacientesPagedQuery"/>.</summary>
public sealed class GetPacientesPagedQueryHandler : IRequestHandler<GetPacientesPagedQuery, PagedResult<GetPacienteDto>>
{
    private readonly IPacienteRepository _pacienteRepository;

    public GetPacientesPagedQueryHandler(IPacienteRepository pacienteRepository)
    {
        _pacienteRepository = pacienteRepository;
    }

    public async Task<PagedResult<GetPacienteDto>> Handle(GetPacientesPagedQuery request, CancellationToken cancellationToken)
    {
        var result = await _pacienteRepository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            string.IsNullOrWhiteSpace(request.Filter) ? null : Filter.FromStringExpression<Paciente>(request.Filter),
            orderBy: request.OrderBy,
            asNoTracking: true,
            cancellationToken: cancellationToken,
            includes: p => p.Historiales);

        return new PagedResult<GetPacienteDto>
        {
            Data = result.Data.Select(PacienteMappings.ToDto).ToList(),
            TotalRecords = result.TotalRecords,
            PageSize = result.PageSize,
            CurrentPage = result.CurrentPage
        };
    }
}
