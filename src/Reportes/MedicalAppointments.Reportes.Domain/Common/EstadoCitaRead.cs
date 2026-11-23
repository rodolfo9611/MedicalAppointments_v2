namespace MedicalAppointments.Reportes.Domain.Common;

/// <summary>
/// Catálogo de estados de cita utilizado por los reportes.
/// </summary>
public enum EstadoCitaRead
{
    /// <summary>Cita agendada.</summary>
    Agendada = 1,

    /// <summary>Paciente citado en consulta.</summary>
    Citada = 2,

    /// <summary>Cita pendiente de resultado.</summary>
    PendienteResultados = 3,

    /// <summary>Cita concluida.</summary>
    Concluida = 4,

    /// <summary>Cita cancelada.</summary>
    Cancelada = 5
}
