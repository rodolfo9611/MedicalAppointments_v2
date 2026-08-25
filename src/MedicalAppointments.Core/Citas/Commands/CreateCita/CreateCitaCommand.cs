using MediatR;

namespace MedicalAppointments.Core.Citas.Commands.CreateCita;

public record CreateCitaCommand(
    long PacienteID,
    int MedicoID,
    int? EspecialidadID,
    int? ConsultorioID,
    int EstadoCitaID,
    DateTime FechaHoraInicio,
    DateTime? FechaHoraFin,
    string? Motivo,
    string? Observaciones,
    int? UsuarioCreacionID) : IRequest<long>;