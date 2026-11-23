namespace MedicalAppointments.Domain.Citas.Integration;

/// <summary>
/// Contrato de integración para registrar un evento en el historial clínico
/// del paciente. No es una entidad de dominio de Citas.
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
