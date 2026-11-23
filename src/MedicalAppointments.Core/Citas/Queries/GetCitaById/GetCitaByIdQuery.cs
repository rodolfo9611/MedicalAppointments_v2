using MediatR;
using MedicalAppointments.Core.Citas.DTOs;

namespace MedicalAppointments.Core.Citas.Queries.GetCitaById;

public record GetCitaByIdQuery(long CitaID) : IRequest<GetCitaDto?>;
