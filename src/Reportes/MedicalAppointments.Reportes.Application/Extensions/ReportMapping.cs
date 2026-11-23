using MedicalAppointments.Reportes.Application.DTOs;
using MedicalAppointments.Reportes.Domain.DTOs;

namespace MedicalAppointments.Reportes.Application.Extensions;

/// <summary>
/// Conversiones de los DTOs de lectura del dominio a los DTOs públicos de reportes.
/// </summary>
internal static class ReportMapping
{
    /// <summary>Convierte un paciente leído por HTTP en un registro de reporte.</summary>
    public static PacienteReportDto ToReportDto(this PacienteReadDto source) => new()
    {
        PacienteID = source.Id,
        NombreCompleto = string.Join(" ", new[] { source.Nombre, source.Apellido }
            .Where(part => !string.IsNullOrWhiteSpace(part))),
        DocumentoIdentidad = source.DocumentoIdentidad,
        FechaNacimiento = source.FechaNacimiento
    };

    /// <summary>Convierte una cita leída por HTTP en un registro de reporte.</summary>
    public static CitaReportDto ToReportDto(this CitaReadDto source) => new()
    {
        CitaID = source.CitaID,
        PacienteID = source.PacienteID,
        EstadoCitaID = source.EstadoCitaID,
        Estado = source.Estado,
        FechaHoraInicio = source.FechaHoraInicio,
        NombreMedico = source.NombreMedico,
        Especialidad = source.Especialidad
    };

    /// <summary>Convierte una receta leída por HTTP en un registro de reporte.</summary>
    public static RecetaReportDto ToReportDto(this RecetaReadDto source) => new()
    {
        RecetaMedicaID = source.RecetaMedicaID,
        CitaID = source.CitaID,
        PacienteID = source.PacienteID,
        NombrePaciente = source.NombrePaciente,
        Diagnostico = source.Diagnostico,
        FechaEmision = source.FechaEmision,
        EsAnulada = source.EsAnulada
    };

    /// <summary>Convierte una notificación leída por HTTP en un registro de reporte.</summary>
    public static NotificacionReportDto ToReportDto(this NotificacionReadDto source) => new()
    {
        NotificacionID = source.NotificacionID,
        CitaID = source.CitaID,
        PacienteID = source.PacienteID,
        PacienteNombre = source.PacienteNombre,
        Tipo = source.Tipo,
        Estado = source.Estado,
        Canal = source.Canal,
        FechaRegistro = source.FechaRegistro,
        FechaEnvio = source.FechaEnvio
    };
}
