using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
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

    public async Task<HttpResponse<PagedDto<List<GetCitaDto>>>> Handle(
        GetCitasPagedQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            Expression<Func<Cita, bool>>? filter = null;
            if (!string.IsNullOrWhiteSpace(request.Filter))
                filter = Filter.FromStringExpression<Cita>(request.Filter);

            var result = await _citaRepository.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                filter,
                orderBy: request.OrderBy,
                asNoTracking: true,
                splitQuery: false,
                cancellationToken: cancellationToken);

            var data = result.Data.Select(c => c.ToDto()).ToList();

            return new HttpResponse<PagedDto<List<GetCitaDto>>>
            {
                Success = true,
                Message = "Consulta paginada generica exitosa.",
                Data = new PagedDto<List<GetCitaDto>>
                {
                    TotalRecords = result.TotalRecords,
                    TotalPage = result.TotalPage,
                    CurrentPage = result.CurrentPage,
                    PageSize = result.PageSize,
                    Data = data
                }
            };
        }
        catch (ValidationException ex)
        {
            return new HttpResponse<PagedDto<List<GetCitaDto>>>
            {
                Success = false,
                Message = ex.Message
            };
        }
    }
}