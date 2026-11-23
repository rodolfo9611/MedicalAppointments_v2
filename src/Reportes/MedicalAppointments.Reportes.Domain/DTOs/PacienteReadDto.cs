namespace MedicalAppointments.Reportes.Domain.DTOs;

/// <summary>
/// Proyección de solo lectura de un paciente obtenida por HTTP del microservicio de pacientes.
/// </summary>
public sealed class PacienteReadDto
{
    /// <summary>Identificador del paciente.</summary>
    public int Id { get; set; }

    /// <summary>
    /// Alias del identificador para soportar contratos que exponen <c>PacienteID</c>.
    /// </summary>
    public int PacienteID
    {
        get => Id;
        set => Id = value;
    }

    /// <summary>Nombre del paciente.</summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Apellido del paciente.</summary>
    public string Apellido { get; set; } = string.Empty;

    /// <summary>Documento de identidad del paciente.</summary>
    public string DocumentoIdentidad { get; set; } = string.Empty;

    /// <summary>Fecha de nacimiento del paciente.</summary>
    public DateTime FechaNacimiento { get; set; }
}
