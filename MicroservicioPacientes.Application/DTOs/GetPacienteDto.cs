namespace MicroservicioPacientes.Application.DTOs;

/// <summary>
/// Representación de un paciente devuelta por la API.
/// </summary>
public class GetPacienteDto
{
    /// <summary>Identificador del paciente (alias de Id).</summary>
    public int PacienteID { get; set; }

    /// <summary>Nombre del paciente.</summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Apellido del paciente.</summary>
    public string Apellido { get; set; } = string.Empty;

    /// <summary>Nombre completo calculado, no persistido.</summary>
    public string NombreCompleto => $"{Nombre} {Apellido}".Trim();

    /// <summary>Documento de identidad del paciente.</summary>
    public string DocumentoIdentidad { get; set; } = string.Empty;

    /// <summary>
    /// Alias público del documento usado por el resto de microservicios y por el frontend.
    /// </summary>
    public string Documento
    {
        get => DocumentoIdentidad;
        set => DocumentoIdentidad = value;
    }

    /// <summary>Fecha de nacimiento del paciente.</summary>
    public DateTime FechaNacimiento { get; set; }

    /// <summary>Código único del paciente.</summary>
    public string CodigoPaciente { get; set; } = string.Empty;

    /// <summary>Correo electrónico del paciente.</summary>
    public string? CorreoElectronico { get; set; }

    /// <summary>Teléfono del paciente.</summary>
    public string? Telefono { get; set; }

    /// <summary>Eventos del historial clínico del paciente.</summary>
    public List<GetHistorialMedicoDto> Historiales { get; set; } = new();
}
