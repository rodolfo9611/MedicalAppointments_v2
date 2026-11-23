using MediatR;
using MedicalAppointments.RecetasMedicas.Application.Common;
using MedicalAppointments.RecetasMedicas.Application.DTOs;
using MedicalAppointments.RecetasMedicas.Domain.Interfaces;

namespace MedicalAppointments.RecetasMedicas.Application.Queries.GetRecetaActivaByCita;

/// <summary>
/// Receta activa de una cita. Permite al frontend decidir entre
/// "Emitir Receta" y "Ver/Imprimir Receta".
/// </summary>
public sealed record GetRecetaActivaByCitaQuery(long CitaID) : IRequest<RecetaMedicaDto?>;

public sealed class GetRecetaActivaByCitaQueryHandler : IRequestHandler<GetRecetaActivaByCitaQuery, RecetaMedicaDto?>
{
    private readonly IRecetaMedicaRepository _repository;

    public GetRecetaActivaByCitaQueryHandler(IRecetaMedicaRepository repository) => _repository = repository;

    public async Task<RecetaMedicaDto?> Handle(
        GetRecetaActivaByCitaQuery request,
        CancellationToken cancellationToken)
    {
        var receta = await _repository.GetActivaByCitaIdAsync(request.CitaID, cancellationToken);
        return receta?.ToDto();
    }
}
