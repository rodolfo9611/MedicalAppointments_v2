using MediatR;
using MedicalAppointments.Domain.Citas.Interfaces;

namespace MedicalAppointments.Core.Citas.Commands.CreateCitas;

public class CreateCitasCommandHandler : IRequestHandler<CreateCitasCommand, int>
{
    private readonly ICitaRepository _citaRepository;

    public CreateCitasCommandHandler(ICitaRepository citaRepository)
    {
        _citaRepository = citaRepository;
    }

    public async Task<int> Handle(CreateCitasCommand request, CancellationToken cancellationToken)
    {
        await _citaRepository.AddRangeAsync(request.Citas, cancellationToken);
        return request.Citas.Count;
    }
}