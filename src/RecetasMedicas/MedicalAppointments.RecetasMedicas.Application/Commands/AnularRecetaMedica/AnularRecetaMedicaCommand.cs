using MediatR;
using MedicalAppointments.RecetasMedicas.Application.Common;
using MedicalAppointments.RecetasMedicas.Application.DTOs;

namespace MedicalAppointments.RecetasMedicas.Application.Commands.AnularRecetaMedica;

/// <summary>Anulación lógica: nunca elimina físicamente la receta.</summary>
public sealed record AnularRecetaMedicaCommand(long RecetaMedicaID, string? Motivo)
    : IRequest<ApplicationResult<RecetaMedicaDto>>;
