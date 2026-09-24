using MediatR;
using MedicalAppointments.Core.Citas.DTOs;
using Nuget.Persistence.Common;

namespace MedicalAppointments.Core.Citas.Queries.GetCitaOneBy;

public record GetCitaOneByQuery(string Filter) : IRequest<HttpResponse<GetCitaDto?>>;