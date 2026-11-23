using MediatR;
using MedicalAppointments.RecetasMedicas.Application.Common;
using MedicalAppointments.RecetasMedicas.Application.DTOs;

namespace MedicalAppointments.RecetasMedicas.Application.Commands.CreateRecetaMedica;

/// <summary>
/// Única forma de crear una receta: siempre asociada a una cita concluida.
/// El cuerpo no incluye datos de paciente, cita, médico ni fecha.
/// </summary>
public sealed record CreateRecetaMedicaCommand(
    long CitaID,
    string Diagnostico,
    string Indicaciones,
    string Tratamiento,
    string? Observaciones) : IRequest<ApplicationResult<RecetaMedicaDto>>;
