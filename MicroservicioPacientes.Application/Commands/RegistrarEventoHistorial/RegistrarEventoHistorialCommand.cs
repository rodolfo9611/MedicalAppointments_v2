using MediatR;
using MicroservicioPacientes.Application.DTOs;
using MicroservicioPacientes.Application.Exceptions;
using MicroservicioPacientes.Domain;

namespace MicroservicioPacientes.Application.Commands.RegistrarEventoHistorial;

/// <summary>
/// Registra un evento en el historial clínico. Devuelve false cuando el EventKey
/// ya existe, para garantizar idempotencia.
/// </summary>
public sealed record RegistrarEventoHistorialCommand(
    int PacienteId,
    EventoHistorialRequestDto Evento) : IRequest<bool>;

/// <summary>Manejador de <see cref="RegistrarEventoHistorialCommand"/>.</summary>
public sealed class RegistrarEventoHistorialCommandHandler : IRequestHandler<RegistrarEventoHistorialCommand, bool>
{
    private readonly IPacienteRepository _pacienteRepository;

    public RegistrarEventoHistorialCommandHandler(IPacienteRepository pacienteRepository)
    {
        _pacienteRepository = pacienteRepository;
    }

    public async Task<bool> Handle(RegistrarEventoHistorialCommand request, CancellationToken cancellationToken)
    {
        var paciente = await _pacienteRepository.GetByIdAsync(request.PacienteId);
        if (paciente is null)
            throw new PacienteNotFoundException(request.PacienteId);

        var eventKey = request.Evento.EventKey.Trim();
        var existente = await _pacienteRepository.GetEventoByKeyAsync(eventKey, cancellationToken);
        if (existente is not null)
            return false;

        var ahora = DateTime.Now;
        var evento = new HistorialMedico
        {
            PacienteId = request.PacienteId,
            TipoEvento = request.Evento.TipoEvento,
            Descripcion = request.Evento.Descripcion,
            Diagnostico = request.Evento.Descripcion,
            Tratamiento = string.Empty,
            EventKey = eventKey,
            FechaEvento = request.Evento.FechaEvento ?? ahora,
            FechaRegistro = ahora,
            CitaID = request.Evento.CitaID,
            EstadoCitaID = request.Evento.EstadoCitaID,
            RecetaMedicaID = request.Evento.RecetaMedicaID,
            NotificacionID = request.Evento.NotificacionID,
            SecuenciaAtencion = request.Evento.SecuenciaAtencion,
            FechaAnterior = request.Evento.FechaAnterior,
            FechaNueva = request.Evento.FechaNueva,
            MotivoReprogramacion = request.Evento.MotivoReprogramacion
        };

        await _pacienteRepository.AddEventoAsync(evento, cancellationToken);
        return true;
    }
}
