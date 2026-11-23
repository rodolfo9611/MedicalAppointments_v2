using MediatR;
using MedicalAppointments.RecetasMedicas.Application.Common;
using MedicalAppointments.RecetasMedicas.Application.DTOs;
using MedicalAppointments.RecetasMedicas.Domain.Interfaces;
using Nuget.Persistence.Abstractions;

namespace MedicalAppointments.RecetasMedicas.Application.Queries.GetRecetasPagedReport;

/// <summary>Consulta paginada y filtrada para el microservicio de Reportes.</summary>
public sealed class GetRecetasPagedReportQuery : IRequest<PagedResult<RecetaMedicaDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public long? PacienteID { get; set; }
    public string? Diagnostico { get; set; }
    public DateTime? FechaDesde { get; set; }
    public DateTime? FechaHasta { get; set; }
    public bool IncluirAnuladas { get; set; }
}

public sealed class GetRecetasPagedReportQueryHandler
    : IRequestHandler<GetRecetasPagedReportQuery, PagedResult<RecetaMedicaDto>>
{
    private readonly IRecetaMedicaRepository _repository;

    public GetRecetasPagedReportQueryHandler(IRecetaMedicaRepository repository) => _repository = repository;

    public async Task<PagedResult<RecetaMedicaDto>> Handle(
        GetRecetasPagedReportQuery request,
        CancellationToken cancellationToken)
    {
        DateTime? fechaHasta = request.FechaHasta;
        if (fechaHasta.HasValue && fechaHasta.Value == fechaHasta.Value.Date)
        {
            fechaHasta = fechaHasta.Value.Date.AddDays(1).AddTicks(-1);
        }

        var pagina = await _repository.GetPagedFilteredAsync(
            request.PageNumber,
            request.PageSize,
            request.PacienteID,
            request.Diagnostico,
            request.FechaDesde,
            fechaHasta,
            request.IncluirAnuladas,
            cancellationToken);

        return pagina.ToPagedDto();
    }
}
