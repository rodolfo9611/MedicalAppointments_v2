using MediatR;
using MedicalAppointments.Domain.Citas.Entities;
using MedicalAppointments.Domain.Citas.Enums;
using MedicalAppointments.Domain.Citas.Integration;
using MedicalAppointments.Domain.Citas.Interfaces;

namespace MedicalAppointments.Core.Citas.Commands.CreateCita;

public class CreateCitaCommandHandler : IRequestHandler<CreateCitaCommand, long>
{
    private readonly ICitaRepository _citaRepository;
    private readonly IHistorialClinicoClient _historialClinicoClient;

    public CreateCitaCommandHandler(ICitaRepository citaRepository, IHistorialClinicoClient historialClinicoClient)
    {
        _citaRepository = citaRepository;
        _historialClinicoClient = historialClinicoClient;
    }

    public async Task<long> Handle(CreateCitaCommand request, CancellationToken cancellationToken)
    {
        Validar(request);

        var cita = new Cita
        {
            PacienteID = request.PacienteID,
            MedicoID = request.MedicoID,
            EspecialidadID = request.EspecialidadID,
            ConsultorioID = request.ConsultorioID,
            EstadoCitaID = (int)EstadoCitaEnum.Agendada,
            FechaHoraInicio = request.FechaHoraInicio,
            FechaHoraFin = request.FechaHoraFin,
            Motivo = request.Motivo,
            Observaciones = request.Observaciones,
            NombreMedico = request.NombreMedico,
            Especialidad = request.Especialidad,
            UsuarioCreacionID = request.UsuarioCreacionID,
            FechaCreacion = DateTime.UtcNow
        };

        var id = await _citaRepository.AddAsync(cita);

        await _historialClinicoClient.RegistrarEventoAsync(new EventoHistorialRequest(
            PacienteID: cita.PacienteID,
            TipoEvento: "CitaCreada",
            Descripcion: $"Cita #{id} agendada para el {cita.FechaHoraInicio:dd/MM/yyyy HH:mm}. Motivo: {cita.Motivo ?? "No especificado"}.",
            EventKey: $"CITA:{id}:CREACION",
            FechaEvento: cita.FechaCreacion,
            CitaID: id,
            EstadoCitaID: cita.EstadoCitaID), cancellationToken);

        return id;
    }

    private static void Validar(CreateCitaCommand request)
    {
        if (request.PacienteID <= 0)
            throw new ArgumentException("El PacienteID debe ser mayor a 0.");
        if (request.MedicoID <= 0)
            throw new ArgumentException("El MedicoID debe ser mayor a 0.");
        if (request.FechaHoraInicio == default)
            throw new ArgumentException("La fecha y hora de inicio son obligatorias.");
        if (request.FechaHoraFin.HasValue && request.FechaHoraFin <= request.FechaHoraInicio)
            throw new ArgumentException("La fecha de fin debe ser posterior a la fecha de inicio.");
        if (!Enum.IsDefined(typeof(EstadoCitaEnum), request.EstadoCitaID))
            throw new ArgumentException("El estado de la cita no es válido.");
        if (request.Motivo is { Length: > 1000 })
            throw new ArgumentException("El motivo no puede superar los 1000 caracteres.");
        if (request.Observaciones is { Length: > 1000 })
            throw new ArgumentException("Las observaciones no pueden superar los 1000 caracteres.");
        if (request.NombreMedico is { Length: > 150 } || request.Especialidad is { Length: > 150 })
            throw new ArgumentException("NombreMedico y Especialidad no pueden superar los 150 caracteres.");
    }
}
