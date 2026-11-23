using MediatR;
using MicroservicioPacientes.Application.DTOs;
using MicroservicioPacientes.Application.Mappings;
using Nuget.Persistence.Abstractions;

namespace MicroservicioPacientes.Application.Queries.GetHistorialMedico;

/// <summary>Consulta paginada del historial clínico de un paciente.</summary>
public sealed record GetHistorialMedicoQuery(
    int PacienteId,
    string? TipoEvento,
    DateTime? FechaDesde,
    DateTime? FechaHasta,
    int PageNumber,
    int PageSize) : IRequest<PagedResult<GetHistorialMedicoDto>?>;

/// <summary>Manejador de <see cref="GetHistorialMedicoQuery"/>.</summary>
public sealed class GetHistorialMedicoQueryHandler : IRequestHandler<GetHistorialMedicoQuery, PagedResult<GetHistorialMedicoDto>?>
{
    private readonly IPacienteRepository _pacienteRepository;

    public GetHistorialMedicoQueryHandler(IPacienteRepository pacienteRepository)
    {
        _pacienteRepository = pacienteRepository;
    }

    public async Task<PagedResult<GetHistorialMedicoDto>?> Handle(GetHistorialMedicoQuery request, CancellationToken cancellationToken)
    {
        var paciente = await _pacienteRepository.GetByIdAsync(request.PacienteId);
        if (paciente is null)
            return null;

        var result = await _pacienteRepository.GetHistorialPagedAsync(
            request.PacienteId,
            request.TipoEvento,
            request.FechaDesde,
            request.FechaHasta,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        return new PagedResult<GetHistorialMedicoDto>
        {
            Data = result.Data.Select(PacienteMappings.ToDto).ToList(),
            TotalRecords = result.TotalRecords,
            PageSize = result.PageSize,
            CurrentPage = result.CurrentPage
        };
    }
}
