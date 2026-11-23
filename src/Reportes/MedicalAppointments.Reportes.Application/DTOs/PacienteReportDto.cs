namespace MedicalAppointments.Reportes.Application.DTOs;

/// <summary>Registro de paciente devuelto por el reporte de pacientes.</summary>
public sealed class PacienteReportDto
{
    /// <summary>Identificador del paciente.</summary>
    public int PacienteID { get; set; }

    /// <summary>Nombre completo del paciente.</summary>
    public string NombreCompleto { get; set; } = string.Empty;

    /// <summary>Documento de identidad del paciente.</summary>
    public string DocumentoIdentidad { get; set; } = string.Empty;

    /// <summary>Fecha de nacimiento del paciente.</summary>
    public DateTime FechaNacimiento { get; set; }
}
