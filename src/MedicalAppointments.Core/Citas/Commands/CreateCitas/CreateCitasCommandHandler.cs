using MediatR;
using MedicalAppointments.Domain.Citas.Enums;
using MedicalAppointments.Domain.Citas.Integration;
using MedicalAppointments.Domain.Citas.Interfaces;

namespace MedicalAppointments.Core.Citas.Commands.CreateCitas;

public class CreateCitasCommandHandler : IRequestHandler<CreateCitasCommand, int>
{
    private readonly ICitaRepository _citaRepository;
    private readonly IHistorialClinicoClient _historialClinicoClient;

    public CreateCitasCommandHandler(ICitaRepository citaRepository, IHistorialClinicoClient historialClinicoClient)
    {
        _citaRepository = citaRepository;
        _historialClinicoClient = historialClinicoClient;
    }

    public async Task<int> Handle(CreateCitasCommand request, CancellationToken cancellationToken)
    {
        if (request.Citas is null || request.Citas.Count == 0)
            return 0;

        var fechaCreacion = DateTime.UtcNow;
        foreach (var cita in request.Citas)
        {
            if (cita.PacienteID <= 0 || cita.MedicoID <= 0)
                throw new ArgumentException("Todas las citas deben tener PacienteID y MedicoID válidos.");

            cita.EstadoCitaID = (int)EstadoCitaEnum.Agendada;
            if (cita.FechaCreacion == default)
                cita.FechaCreacion = fechaCreacion;
        }

        await _citaRepository.AddRangeAsync(request.Citas, cancellationToken);

        foreach (var cita in request.Citas)
        {
            await _historialClinicoClient.RegistrarEventoAsync(new EventoHistorialRequest(
                PacienteID: cita.PacienteID,
                TipoEvento: "CitaCreada",
                Descripcion: $"Cita #{cita.CitaID} agendada para el {cita.FechaHoraInicio:dd/MM/yyyy HH:mm}.",
                EventKey: $"CITA:{cita.CitaID}:CREACION",
                FechaEvento: fechaCreacion,
                CitaID: cita.CitaID,
                EstadoCitaID: cita.EstadoCitaID), cancellationToken);
        }

        return request.Citas.Count;
    }
}
