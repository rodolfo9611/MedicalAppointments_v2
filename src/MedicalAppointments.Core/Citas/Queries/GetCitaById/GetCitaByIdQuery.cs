using MediatR;
using MedicalAppointments.Domain.Citas.Entities;

namespace MedicalAppointments.Core.Citas.Queries.GetCitaById;

public record GetCitaByIdQuery(long CitaID) : IRequest<Cita?>;  