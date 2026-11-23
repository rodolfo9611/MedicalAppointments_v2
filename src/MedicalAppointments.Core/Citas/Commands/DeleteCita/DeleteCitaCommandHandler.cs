using MediatR;
using MedicalAppointments.Domain.Citas.Enums;
using MedicalAppointments.Domain.Citas.Integration;
using MedicalAppointments.Domain.Citas.Interfaces;

namespace MedicalAppointments.Core.Citas.Commands.DeleteCita;

public class DeleteCitaCommandHandler : IRequestHandler<DeleteCitaCommand, bool>
{
    private readonly ICitaRepository _citaRepository;
    private readonly IHistorialClinicoClient _historialClinicoClient;

    public DeleteCitaCommandHandler(ICitaRepository citaRepository, IHistorialClinicoClient historialClinicoClient)
    {
        _citaRepository = citaRepository;
        _historialClinicoClient = historialClinicoClient;
    }

    public async Task<bool> Handle(DeleteCitaCommand request, CancellationToken cancellationToken)
    {
        var cita = await _citaRepository.GetByIdAsync(request.CitaID);
        if (cita is null)
            return false;

        if (cita.EstadoCitaID == (int)EstadoCitaEnum.Cancelada)
            return true;

        cita.EstadoCitaID = (int)EstadoCitaEnum.Cancelada;
        await _citaRepository.UpdateAsync(cita);

        await _historialClinicoClient.RegistrarEventoAsync(new EventoHistorialRequest(
            PacienteID: cita.PacienteID,
            TipoEvento: "CambioEstado",
            Descripcion: $"La cita #{cita.CitaID} fue cancelada.",
            EventKey: $"ESTADO:{cita.CitaID}:{(int)EstadoCitaEnum.Cancelada}:{DateTime.UtcNow:yyyyMMddHHmmss}",
            FechaEvento: DateTime.UtcNow,
            CitaID: cita.CitaID,
            EstadoCitaID: (int)EstadoCitaEnum.Cancelada), cancellationToken);

        return true;
    }
}
