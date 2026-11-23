using MediatR;
using MedicalAppointments.Core.Citas.DTOs;
using MedicalAppointments.Domain.Citas.Interfaces;

namespace MedicalAppointments.Core.Citas.Queries.GetCitaById;

public class GetCitaByIdQueryHandler : IRequestHandler<GetCitaByIdQuery, GetCitaDto?>
{
    private readonly ICitaRepository _citaRepository;

    public GetCitaByIdQueryHandler(ICitaRepository citaRepository)
    {
        _citaRepository = citaRepository;
    }

    public async Task<GetCitaDto?> Handle(GetCitaByIdQuery request, CancellationToken cancellationToken)
    {
        var cita = await _citaRepository.GetByIdAsync(request.CitaID);
        return cita?.ToDto();
    }
}
