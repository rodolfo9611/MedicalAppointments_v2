using MediatR;
using MedicalAppointments.Core.Citas.DTOs;
using Nuget.Persistence.Common;

namespace MedicalAppointments.Core.Citas.Queries.GetCitasPaged;

public class GetCitasPagedQuery : RequestParametersGets, IRequest<HttpResponse<PagedDto<List<GetCitaDto>>>>
{
}