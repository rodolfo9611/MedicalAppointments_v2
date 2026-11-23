using MediatR;
using MedicalAppointments.Core.Citas.DTOs;
using MedicalAppointments.Domain.Citas.Enums;
using MedicalAppointments.Domain.Citas.Integration;
using MedicalAppointments.Domain.Citas.Interfaces;

namespace MedicalAppointments.Core.Citas.Commands.UpdateCita;

public class UpdateCitaCommandHandler : IRequestHandler<UpdateCitaCommand, GetCitaDto?>
{
    private static readonly Dictionary<EstadoCitaEnum, EstadoCitaEnum[]> TransicionesPermitidas = new()
    {
        [EstadoCitaEnum.Agendada] = new[]
        {
            EstadoCitaEnum.Citada,
            EstadoCitaEnum.PendienteResultados,
            EstadoCitaEnum.Concluida,
            EstadoCitaEnum.Cancelada
        },
        [EstadoCitaEnum.Citada] = new[]
        {
            EstadoCitaEnum.PendienteResultados,
            EstadoCitaEnum.Concluida,
            EstadoCitaEnum.Cancelada
        },
        [EstadoCitaEnum.PendienteResultados] = new[]
        {
            EstadoCitaEnum.PendienteResultados,
            EstadoCitaEnum.Citada,
            EstadoCitaEnum.Concluida,
            EstadoCitaEnum.Cancelada
        },
        [EstadoCitaEnum.Concluida] = Array.Empty<EstadoCitaEnum>(),
        [EstadoCitaEnum.Cancelada] = Array.Empty<EstadoCitaEnum>()
    };

    private readonly ICitaRepository _citaRepository;
    private readonly IHistorialClinicoClient _historialClinicoClient;

    public UpdateCitaCommandHandler(ICitaRepository citaRepository, IHistorialClinicoClient historialClinicoClient)
    {
        _citaRepository = citaRepository;
        _historialClinicoClient = historialClinicoClient;
    }

    public async Task<GetCitaDto?> Handle(UpdateCitaCommand request, CancellationToken cancellationToken)
    {
        var cita = await _citaRepository.GetByIdAsync(request.CitaID);
        if (cita is null)
            return null;

        var datos = request.Datos ?? throw new ArgumentException("Los datos de actualización son obligatorios.");
        var estadoActual = Enum.IsDefined(typeof(EstadoCitaEnum), cita.EstadoCitaID)
            ? (EstadoCitaEnum)cita.EstadoCitaID
            : EstadoCitaEnum.Agendada;
        var estadoDestino = datos.Estado;

        if (!Enum.IsDefined(typeof(EstadoCitaEnum), estadoDestino))
            throw new ArgumentException("El estado de destino no es válido.");

        if (!TransicionesPermitidas[estadoActual].Contains(estadoDestino))
            throw new InvalidOperationException(
                $"La transición de {estadoActual} a {estadoDestino} no está permitida para el mismo CitaID.");

        var esSalidaPrimeraConsulta = estadoActual == EstadoCitaEnum.Agendada &&
                                      estadoDestino != EstadoCitaEnum.Cancelada;
        if (esSalidaPrimeraConsulta && datos.RegistroAtencion is null)
            throw new InvalidOperationException(
                "Debe registrarse una atención médica antes de cambiar el estado de una cita Agendada.");

        if (datos.Reprogramacion is not null)
        {
            if (estadoActual != EstadoCitaEnum.PendienteResultados || estadoDestino != EstadoCitaEnum.PendienteResultados)
                throw new InvalidOperationException(
                    "La reprogramación solo es válida cuando la cita permanece en PendienteResultados.");

            if (string.IsNullOrWhiteSpace(datos.Reprogramacion.Motivo))
                throw new ArgumentException("El motivo de la reprogramación es obligatorio.");
        }

        var registroAtencion = datos.RegistroAtencion;
        if (registroAtencion is not null)
        {
            if (registroAtencion.Secuencia < 1)
                throw new ArgumentException("La secuencia de atención debe ser mayor o igual a 1.");

            var secuenciaEsperada = await _historialClinicoClient
                .ObtenerSiguienteSecuenciaAtencionAsync(cita.CitaID, cancellationToken);

            var esReintento = registroAtencion.Secuencia < secuenciaEsperada &&
                              await _historialClinicoClient.ExisteAtencionAsync(
                                  cita.CitaID, registroAtencion.Secuencia, cancellationToken);

            if (registroAtencion.Secuencia != secuenciaEsperada && !esReintento)
                throw new InvalidOperationException(
                    $"La secuencia de atención esperada para la cita #{cita.CitaID} es {secuenciaEsperada}.");

            if (!esReintento)
            {
                await _historialClinicoClient.RegistrarEventoAsync(new EventoHistorialRequest(
                    PacienteID: cita.PacienteID,
                    TipoEvento: "AtencionRegistrada",
                    Descripcion: registroAtencion.Observaciones is null
                        ? $"Atención #{registroAtencion.Secuencia} registrada en la cita #{cita.CitaID}."
                        : $"Atención #{registroAtencion.Secuencia} registrada en la cita #{cita.CitaID}. {registroAtencion.Observaciones}",
                    EventKey: $"ATENCION:{cita.CitaID}:{registroAtencion.Secuencia}",
                    FechaEvento: registroAtencion.FechaAtencion ?? DateTime.UtcNow,
                    CitaID: cita.CitaID,
                    EstadoCitaID: cita.EstadoCitaID,
                    SecuenciaAtencion: registroAtencion.Secuencia), cancellationToken);
            }
        }

        if (estadoDestino == EstadoCitaEnum.Concluida)
        {
            var existeAtencion = await _historialClinicoClient.ExisteAtencionAsync(cita.CitaID, cancellationToken);
            if (!existeAtencion)
                throw new InvalidOperationException(
                    "No se puede concluir una cita sin haberse registrado una atención médica.");
        }

        var fechaAnterior = cita.FechaHoraInicio;

        cita.EstadoCitaID = (int)estadoDestino;
        if (datos.Reprogramacion is not null)
        {
            cita.FechaHoraInicio = datos.Reprogramacion.NuevaFechaHoraInicio;
            cita.FechaHoraFin = datos.Reprogramacion.NuevaFechaHoraFin ?? cita.FechaHoraFin;
        }
        else
        {
            if (datos.FechaHoraInicio.HasValue)
                cita.FechaHoraInicio = datos.FechaHoraInicio.Value;
            if (datos.FechaHoraFin.HasValue)
                cita.FechaHoraFin = datos.FechaHoraFin.Value;
        }

        if (!string.IsNullOrWhiteSpace(datos.Motivo))
            cita.Motivo = datos.Motivo;
        if (!string.IsNullOrWhiteSpace(datos.Observaciones))
            cita.Observaciones = datos.Observaciones;

        await _citaRepository.UpdateAsync(cita);

        if (estadoActual != estadoDestino)
        {
            await _historialClinicoClient.RegistrarEventoAsync(new EventoHistorialRequest(
                PacienteID: cita.PacienteID,
                TipoEvento: "CambioEstado",
                Descripcion: $"La cita #{cita.CitaID} cambió de {estadoActual} a {estadoDestino}.",
                EventKey: $"ESTADO:{cita.CitaID}:{(int)estadoDestino}:{DateTime.UtcNow:yyyyMMddHHmmss}",
                FechaEvento: DateTime.UtcNow,
                CitaID: cita.CitaID,
                EstadoCitaID: (int)estadoDestino), cancellationToken);
        }

        if (datos.Reprogramacion is not null)
        {
            await _historialClinicoClient.RegistrarEventoAsync(new EventoHistorialRequest(
                PacienteID: cita.PacienteID,
                TipoEvento: "Reprogramacion",
                Descripcion: datos.Reprogramacion.Motivo,
                EventKey: $"REPROGRAMACION:{cita.CitaID}:{DateTime.UtcNow:yyyyMMddHHmmss}",
                FechaEvento: DateTime.UtcNow,
                CitaID: cita.CitaID,
                EstadoCitaID: cita.EstadoCitaID,
                FechaAnterior: fechaAnterior,
                FechaNueva: cita.FechaHoraInicio,
                MotivoReprogramacion: datos.Reprogramacion.Motivo), cancellationToken);
        }

        return cita.ToDto();
    }
}
