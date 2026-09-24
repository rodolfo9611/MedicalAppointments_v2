using MediatR;
using MedicalAppointments.Domain.Citas.Entities;

namespace MedicalAppointments.Core.Citas.Commands.CreateCitas;

public record CreateCitasCommand(List<Cita> Citas) : IRequest<int>;