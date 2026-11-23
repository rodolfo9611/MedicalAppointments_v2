using MediatR;
using MedicalAppointments.RecetasMedicas.Application.Common;
using MedicalAppointments.RecetasMedicas.Application.DTOs;
using MedicalAppointments.RecetasMedicas.Domain.Entities;
using MedicalAppointments.RecetasMedicas.Domain.Integration;
using MedicalAppointments.RecetasMedicas.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace MedicalAppointments.RecetasMedicas.Application.Commands.CreateRecetaMedica;

public sealed class CreateRecetaMedicaCommandHandler
    : IRequestHandler<CreateRecetaMedicaCommand, ApplicationResult<RecetaMedicaDto>>
{
    private const int EstadoConcluida = 4;

    private readonly IRecetaMedicaRepository _repository;
    private readonly ICitasApiClient _citasApiClient;
    private readonly IPacientesApiClient _pacientesApiClient;
    private readonly INotificacionesApiClient _notificacionesApiClient;
    private readonly ILogger<CreateRecetaMedicaCommandHandler> _logger;

    public CreateRecetaMedicaCommandHandler(
        IRecetaMedicaRepository repository,
        ICitasApiClient citasApiClient,
        IPacientesApiClient pacientesApiClient,
        INotificacionesApiClient notificacionesApiClient,
        ILogger<CreateRecetaMedicaCommandHandler> logger)
    {
        _repository = repository;
        _citasApiClient = citasApiClient;
        _pacientesApiClient = pacientesApiClient;
        _notificacionesApiClient = notificacionesApiClient;
        _logger = logger;
    }

    public async Task<ApplicationResult<RecetaMedicaDto>> Handle(
        CreateRecetaMedicaCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Diagnostico) ||
            string.IsNullOrWhiteSpace(request.Indicaciones) ||
            string.IsNullOrWhiteSpace(request.Tratamiento))
        {
            return ApplicationResult<RecetaMedicaDto>.Fail(
                400, "Diagnóstico, indicaciones y tratamiento son obligatorios.");
        }

        var activa = await _repository.GetActivaByCitaIdAsync(request.CitaID, cancellationToken);
        if (activa is not null)
        {
            return ApplicationResult<RecetaMedicaDto>.Fail(
                409,
                $"Ya existe una receta activa para la cita #{request.CitaID} (RecetaMedicaID {activa.RecetaMedicaID}). " +
                "Anule la receta actual antes de emitir una corrección.");
        }

        CitaSnapshotDto? cita;
        try
        {
            cita = await _citasApiClient.GetCitaAsync(request.CitaID, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "No se pudo consultar la cita {CitaID} en la API de Citas.", request.CitaID);
            return ApplicationResult<RecetaMedicaDto>.Fail(
                502, $"No se pudo consultar la cita #{request.CitaID} en la API de Citas: {ex.Message}");
        }

        if (cita is null)
        {
            return ApplicationResult<RecetaMedicaDto>.Fail(404, $"No existe la cita #{request.CitaID}.");
        }

        var estadoEsConcluida = cita.EstadoCitaID == EstadoConcluida ||
            string.Equals(cita.EstadoTexto, "Concluida", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(cita.EstadoNombre, "Concluida", StringComparison.OrdinalIgnoreCase);

        if (!estadoEsConcluida)
        {
            return ApplicationResult<RecetaMedicaDto>.Fail(
                422,
                $"Solo se pueden emitir recetas para citas concluidas. La cita #{request.CitaID} tiene estado " +
                $"'{(string.IsNullOrWhiteSpace(cita.EstadoTexto) ? cita.EstadoCitaID.ToString() : cita.EstadoTexto)}'.");
        }

        IReadOnlyList<int> secuencias;
        try
        {
            secuencias = await _pacientesApiClient.GetSecuenciasAtencionAsync(request.CitaID, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "No se pudieron consultar las atenciones de la cita {CitaID}.", request.CitaID);
            return ApplicationResult<RecetaMedicaDto>.Fail(
                502, $"No se pudieron consultar las atenciones de la cita #{request.CitaID}: {ex.Message}");
        }

        if (secuencias.Count == 0)
        {
            return ApplicationResult<RecetaMedicaDto>.Fail(
                422,
                $"La cita #{request.CitaID} no tiene al menos una atención médica registrada; " +
                "registre la atención antes de emitir la receta.");
        }

        PacienteSnapshotDto? paciente;
        try
        {
            paciente = await _pacientesApiClient.GetPacienteAsync(cita.PacienteID, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "No se pudo consultar el paciente {PacienteID}.", cita.PacienteID);
            return ApplicationResult<RecetaMedicaDto>.Fail(
                502, $"No se pudo consultar el paciente #{cita.PacienteID}: {ex.Message}");
        }

        if (paciente is null)
        {
            return ApplicationResult<RecetaMedicaDto>.Fail(
                404, $"No se encontró el paciente #{cita.PacienteID} asociado a la cita #{request.CitaID}.");
        }

        var recetaAnteriorId = await _repository.GetUltimaAnuladaIdByCitaIdAsync(request.CitaID, cancellationToken);

        var receta = new RecetaMedica
        {
            CitaID = cita.CitaID,
            PacienteID = paciente.PacienteID,
            NombrePaciente = paciente.NombreCompleto,
            FechaConsulta = cita.FechaHoraInicio,
            MedicoID = cita.MedicoID,
            NombreMedico = cita.NombreMedico,
            Especialidad = cita.Especialidad,
            Diagnostico = request.Diagnostico.Trim(),
            Indicaciones = request.Indicaciones.Trim(),
            Tratamiento = request.Tratamiento.Trim(),
            Observaciones = string.IsNullOrWhiteSpace(request.Observaciones) ? null : request.Observaciones.Trim(),
            FechaEmision = DateTime.UtcNow,
            EsAnulada = false,
            RecetaAnteriorID = recetaAnteriorId
        };

        // Persistencia primero: ningún fallo externo puede hacer perder la receta.
        var id = await _repository.AddAsync(receta);
        receta.RecetaMedicaID = id;

        var warnings = new List<string>();

        try
        {
            await _pacientesApiClient.RegistrarEventoAsync(
                new EventoHistorialRequest(
                    PacienteID: receta.PacienteID,
                    TipoEvento: "RecetaEmitida",
                    Descripcion: $"Se emitió la receta médica #{id} para la cita #{receta.CitaID}.",
                    EventKey: $"RECETA:{id}:EMISION",
                    FechaEvento: receta.FechaEmision,
                    CitaID: receta.CitaID,
                    EstadoCitaID: EstadoConcluida,
                    RecetaMedicaID: id,
                    SecuenciaAtencion: secuencias.Max()),
                cancellationToken);
        }
        catch (Exception ex)
        {
            var warning = $"La receta #{id} fue guardada, pero no se pudo registrar el evento 'RecetaEmitida' en el historial: {ex.Message}";
            _logger.LogWarning(ex, "{Warning}", warning);
            warnings.Add(warning);
        }

        try
        {
            var destinatario = !string.IsNullOrWhiteSpace(paciente.CorreoElectronico)
                ? paciente.CorreoElectronico!
                : !string.IsNullOrWhiteSpace(paciente.Telefono)
                    ? paciente.Telefono!
                    : paciente.Documento ?? string.Empty;

            await _notificacionesApiClient.RegistrarNotificacionAsync(
                new CreateNotificacionDto
                {
                    CitaID = receta.CitaID,
                    PacienteID = receta.PacienteID,
                    PacienteNombre = receta.NombrePaciente,
                    Destinatario = destinatario,
                    Canal = !string.IsNullOrWhiteSpace(paciente.CorreoElectronico) ? "Email" : "WhatsApp",
                    Mensaje = $"Se emitió la receta médica #{id} correspondiente a la cita #{receta.CitaID}.",
                    FechaProgramadaCita = receta.FechaConsulta,
                    Estado = "Pendiente",
                    TipoNotificacion = "RecetaEmitida",
                    RecetaMedicaID = id
                },
                cancellationToken);
        }
        catch (Exception ex)
        {
            var warning = $"La receta #{id} fue guardada, pero no se pudo registrar la notificación 'RecetaEmitida': {ex.Message}";
            _logger.LogWarning(ex, "{Warning}", warning);
            warnings.Add(warning);
        }

        var mensaje = warnings.Count == 0
            ? $"Receta médica #{id} emitida correctamente."
            : $"Receta médica #{id} emitida correctamente, con {warnings.Count} advertencia(s) de integración.";

        return ApplicationResult<RecetaMedicaDto>.Ok(receta.ToDto(), 201, mensaje, warnings);
    }
}
