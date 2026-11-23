using Nuget.Persistence.Common;

namespace MedicalAppointments.Reportes.Domain.Requests;

/// <summary>
/// Filtros enviados al cliente HTTP del microservicio de citas.
/// </summary>
public sealed class CitaFilterRequest : RequestParametersGets
{
    /// <summary>Identificador del estado de la cita.</summary>
    public int? EstadoCitaId { get; set; }

    /// <summary>Identificador del paciente.</summary>
    public long? PacienteId { get; set; }

    /// <summary>Fecha y hora inicial del rango.</summary>
    public DateTime? FechaDesde { get; set; }

    /// <summary>Fecha y hora final del rango.</summary>
    public DateTime? FechaHasta { get; set; }
}
