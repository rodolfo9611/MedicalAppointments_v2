using MediatR;
using MedicalAppointments.Domain.Citas.Entities;
using MedicalAppointments.Domain.Citas.Interfaces;

namespace MedicalAppointments.Core.Citas.Queries.GetCitaById;

public class GetCitaByIdQueryHandler : IRequestHandler<GetCitaByIdQuery, Cita?>
{
    private readonly ICitaRepository _citaRepository;

    public GetCitaByIdQueryHandler(ICitaRepository citaRepository)
    {
        _citaRepository = citaRepository;
    }

    public async Task<Cita?> Handle(GetCitaByIdQuery request, CancellationToken cancellationToken)
    {
        return await _citaRepository.GetByIdAsync(request.CitaID);
    }
}