using MediatR;
using MedicalAppointments.Core.Citas.DTOs;
using MedicalAppointments.Domain.Citas.Entities;
using MedicalAppointments.Domain.Citas.Interfaces;
using Nuget.Persistence.Common;

namespace MedicalAppointments.Core.Citas.Queries.GetCitasPaged;

public class GetCitasPagedQueryHandler : IRequestHandler<GetCitasPagedQuery, HttpResponse<PagedDto<List<GetCitaDto>>>>
{
    private readonly ICitaRepository _citaRepository;

    public GetCitasPagedQueryHandler(ICitaRepository citaRepository)
    {
        _citaRepository = citaRepository;
    }

    public async Task<HttpResponse<PagedDto<List<GetCitaDto>>>> Handle(GetCitasPagedQuery request, CancellationToken cancellationToken)
    {
        var filterExpression = !string.IsNullOrWhiteSpace(request.Filter)
            ? Filter.FromStringExpression<Cita>(request.Filter)
            : null;

        var result = await _citaRepository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            filter: filterExpression,
            cancellationToken: cancellationToken);

        var dtoList = result.Data.Select(c => new GetCitaDto
        {
            CitaID = c.CitaID,
            PacienteID = c.PacienteID,
            MedicoID = c.MedicoID,
            EspecialidadID = c.EspecialidadID,
            ConsultorioID = c.ConsultorioID,
            EstadoCitaID = c.EstadoCitaID,
            FechaHoraInicio = c.FechaHoraInicio,
            FechaHoraFin = c.FechaHoraFin,
            Motivo = c.Motivo,
            Observaciones = c.Observaciones,
            UsuarioCreacionID = c.UsuarioCreacionID,
            FechaCreacion = c.FechaCreacion
        }).ToList();

        var pagedDto = new PagedDto<List<GetCitaDto>>
        {
            CurrentPage = result.CurrentPage,
            PageSize = result.PageSize,
            TotalPage = result.TotalPage,
            TotalRecords = result.TotalRecords,
            Data = dtoList
        };

        return new HttpResponse<PagedDto<List<GetCitaDto>>>(pagedDto);
    }
}
