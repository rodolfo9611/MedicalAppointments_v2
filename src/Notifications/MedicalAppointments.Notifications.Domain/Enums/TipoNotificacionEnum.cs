namespace MedicalAppointments.Notifications.Domain.Enums;

/// <summary>
/// Clasificación de la notificación según el evento clínico que la origina.
/// </summary>
public enum TipoNotificacionEnum
{
    RecordatorioCita,
    ReprogramacionCita,
    RecetaEmitida,
    RecetaAnulada
}
