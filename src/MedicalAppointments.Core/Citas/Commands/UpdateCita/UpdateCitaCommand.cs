using MediatR;
using MedicalAppointments.Core.Citas.DTOs;

namespace MedicalAppointments.Core.Citas.Commands.UpdateCita;

public record UpdateCitaCommand(long CitaID, UpdateCitaDto Datos) : IRequest<GetCitaDto?>;
