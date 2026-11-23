using MediatR;
using MedicalAppointments.Core.Citas.DTOs;
using MedicalAppointments.Domain.Citas.Interfaces;

namespace MedicalAppointments.Core.Citas.Queries.GetCitas;

public class GetCitasQueryHandler : IRequestHandler<GetCitasQuery, List<GetCitaDto>>
{
    private readonly ICitaRepository _citaRepository;

    public GetCitasQueryHandler(ICitaRepository citaRepository)
    {
        _citaRepository = citaRepository;
    }

    public async Task<List<GetCitaDto>> Handle(GetCitasQuery request, CancellationToken cancellationToken)
    {
        var citas = await _citaRepository.GetAllAsync(asNoTracking: true);
        return citas.Select(c => c.ToDto()).ToList();
    }
}
