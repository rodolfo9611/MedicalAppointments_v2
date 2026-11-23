namespace MedicalAppointments.RecetasMedicas.Domain.Integration;

/// <summary>
/// Contrato de integración idéntico al utilizado por Citas para
/// POST /api/Pacientes/{id}/historial/eventos.
/// </summary>
public sealed record EventoHistorialRequest(
    long PacienteID,
    string TipoEvento,
    string Descripcion,
    string EventKey,
    DateTime? FechaEvento = null,
    long? CitaID = null,
    int? EstadoCitaID = null,
    long? RecetaMedicaID = null,
    long? NotificacionID = null,
    int? SecuenciaAtencion = null,
    DateTime? FechaAnterior = null,
    DateTime? FechaNueva = null,
    string? MotivoReprogramacion = null);
