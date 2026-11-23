using MediatR;
using MedicalAppointments.RecetasMedicas.Application.Common;
using MedicalAppointments.RecetasMedicas.Application.DTOs;
using MedicalAppointments.RecetasMedicas.Domain.Integration;
using MedicalAppointments.RecetasMedicas.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace MedicalAppointments.RecetasMedicas.Application.Commands.AnularRecetaMedica;

public sealed class AnularRecetaMedicaCommandHandler
    : IRequestHandler<AnularRecetaMedicaCommand, ApplicationResult<RecetaMedicaDto>>
{
    private readonly IRecetaMedicaRepository _repository;
    private readonly IPacientesApiClient _pacientesApiClient;
    private readonly INotificacionesApiClient _notificacionesApiClient;
    private readonly ILogger<AnularRecetaMedicaCommandHandler> _logger;

    public AnularRecetaMedicaCommandHandler(
        IRecetaMedicaRepository repository,
        IPacientesApiClient pacientesApiClient,
        INotificacionesApiClient notificacionesApiClient,
        ILogger<AnularRecetaMedicaCommandHandler> logger)
    {
        _repository = repository;
        _pacientesApiClient = pacientesApiClient;
        _notificacionesApiClient = notificacionesApiClient;
        _logger = logger;
    }

    public async Task<ApplicationResult<RecetaMedicaDto>> Handle(
        AnularRecetaMedicaCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Motivo))
        {
            return ApplicationResult<RecetaMedicaDto>.Fail(
                400, "El motivo de la anulación es obligatorio (query 'motivo' o cuerpo JSON).");
        }

        var receta = await _repository.GetByIdAsync(request.RecetaMedicaID);
        if (receta is null)
        {
            return ApplicationResult<RecetaMedicaDto>.Fail(
                404, $"No existe la receta médica #{request.RecetaMedicaID}.");
        }

        if (receta.EsAnulada)
        {
            return ApplicationResult<RecetaMedicaDto>.Fail(
                409, $"La receta médica #{request.RecetaMedicaID} ya se encuentra anulada.");
        }

        receta.EsAnulada = true;
        receta.FechaAnulacion = DateTime.UtcNow;
        receta.MotivoAnulacion = request.Motivo.Trim();

        // Persistencia primero: los talleres externos son best-effort.
        await _repository.UpdateAsync(receta);

        var warnings = new List<string>();

        try
        {
            await _pacientesApiClient.RegistrarEventoAsync(
                new EventoHistorialRequest(
                    PacienteID: receta.PacienteID,
                    TipoEvento: "RecetaAnulada",
                    Descripcion: $"Se anuló la receta médica #{receta.RecetaMedicaID}. Motivo: {receta.MotivoAnulacion}",
                    EventKey: $"RECETA:{receta.RecetaMedicaID}:ANULACION",
                    FechaEvento: receta.FechaAnulacion,
                    CitaID: receta.CitaID,
                    EstadoCitaID: 4,
                    RecetaMedicaID: receta.RecetaMedicaID),
                cancellationToken);
        }
        catch (Exception ex)
        {
            var warning = $"La receta #{receta.RecetaMedicaID} fue anulada, pero no se pudo registrar el evento 'RecetaAnulada' en el historial: {ex.Message}";
            _logger.LogWarning(ex, "{Warning}", warning);
            warnings.Add(warning);
        }

        try
        {
            var destinatario = await ObtenerDestinatarioAsync(receta.PacienteID, receta.NombrePaciente, cancellationToken);
            await _notificacionesApiClient.RegistrarNotificacionAsync(
                new CreateNotificacionDto
                {
                    CitaID = receta.CitaID,
                    PacienteID = receta.PacienteID,
                    PacienteNombre = receta.NombrePaciente,
                    Destinatario = destinatario,
                    Canal = "Email",
                    Mensaje = $"Se anuló la receta médica #{receta.RecetaMedicaID}. Motivo: {receta.MotivoAnulacion}",
                    FechaProgramadaCita = receta.FechaConsulta,
                    Estado = "Pendiente",
                    TipoNotificacion = "RecetaAnulada",
                    RecetaMedicaID = receta.RecetaMedicaID
                },
                cancellationToken);
        }
        catch (Exception ex)
        {
            var warning = $"La receta #{receta.RecetaMedicaID} fue anulada, pero no se pudo registrar la notificación 'RecetaAnulada': {ex.Message}";
            _logger.LogWarning(ex, "{Warning}", warning);
            warnings.Add(warning);
        }

        var mensaje = warnings.Count == 0
            ? $"Receta médica #{receta.RecetaMedicaID} anulada correctamente."
            : $"Receta médica #{receta.RecetaMedicaID} anulada correctamente, con {warnings.Count} advertencia(s) de integración.";

        return ApplicationResult<RecetaMedicaDto>.Ok(receta.ToDto(), 200, mensaje, warnings);
    }

    private async Task<string> ObtenerDestinatarioAsync(
        long pacienteId,
        string nombrePaciente,
        CancellationToken cancellationToken)
    {
        try
        {
            var paciente = await _pacientesApiClient.GetPacienteAsync(pacienteId, cancellationToken);
            if (paciente is null)
                return nombrePaciente;

            return !string.IsNullOrWhiteSpace(paciente.CorreoElectronico)
                ? paciente.CorreoElectronico!
                : !string.IsNullOrWhiteSpace(paciente.Telefono)
                    ? paciente.Telefono!
                    : paciente.Documento ?? nombrePaciente;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "No se pudo obtener el destinatario de la notificación de anulación para el paciente {PacienteID}.", pacienteId);
            return nombrePaciente;
        }
    }
}
