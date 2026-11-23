using Nuget.Persistence.Common;

namespace MedicalAppointments.Reportes.Domain.Requests;

/// <summary>
/// Filtros enviados al cliente HTTP del microservicio de pacientes.
/// </summary>
public sealed class PacienteFilterRequest : RequestParametersGets
{
    /// <summary>Documento de identidad del paciente.</summary>
    public string? Documento { get; set; }

    /// <summary>Nombre o apellido del paciente.</summary>
    public string? Nombre { get; set; }

    /// <summary>Fecha inicial del rango de nacimiento.</summary>
    public DateTime? FechaDesde { get; set; }

    /// <summary>Fecha final del rango de nacimiento.</summary>
    public DateTime? FechaHasta { get; set; }
}
