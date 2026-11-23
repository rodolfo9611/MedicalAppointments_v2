using MediatR;

namespace MedicalAppointments.Core.Citas.Commands.DeleteCita;

/// <summary>
/// Cancela lógicamente una cita conservando el CitaID y su historial clínico.
/// </summary>
public record DeleteCitaCommand(long CitaID) : IRequest<bool>;
