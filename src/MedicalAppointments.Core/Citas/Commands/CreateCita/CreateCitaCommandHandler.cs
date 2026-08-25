using MediatR;
using MedicalAppointments.Domain.Citas.Entities;
using MedicalAppointments.Domain.Citas.Interfaces;

namespace MedicalAppointments.Core.Citas.Commands.CreateCita;

public class CreateCitaCommandHandler : IRequestHandler<CreateCitaCommand, long>
{
    private readonly ICitaRepository _citaRepository;

    public CreateCitaCommandHandler(ICitaRepository citaRepository)
    {
        _citaRepository = citaRepository;
    }

    public async Task<long> Handle(CreateCitaCommand request, CancellationToken cancellationToken)
    {
        var cita = new Cita
        {
            PacienteID = request.PacienteID,
            MedicoID = request.MedicoID,
            EspecialidadID = request.EspecialidadID,
            ConsultorioID = request.ConsultorioID,
            EstadoCitaID = request.EstadoCitaID,
            FechaHoraInicio = request.FechaHoraInicio,
            FechaHoraFin = request.FechaHoraFin,
            Motivo = request.Motivo,
            Observaciones = request.Observaciones,
            UsuarioCreacionID = request.UsuarioCreacionID,
            FechaCreacion = DateTime.Now
        };

        return await _citaRepository.AddAsync(cita);
    }
}