namespace MicroservicioPacientes.Domain;

/// <summary>
/// Paciente del sistema. El nombre completo se calcula en la capa de aplicación
/// a partir de <see cref="Nombre"/> y <see cref="Apellido"/>; no se persiste como columna.
/// </summary>
public class Paciente
{
    /// <summary>Identificador principal del paciente.</summary>
    public int Id { get; set; }

    /// <summary>Nombre del paciente.</summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Apellido del paciente.</summary>
    public string Apellido { get; set; } = string.Empty;

    /// <summary>Documento de identidad del paciente.</summary>
    public string DocumentoIdentidad { get; set; } = string.Empty;

    /// <summary>Fecha de nacimiento del paciente.</summary>
    public DateTime FechaNacimiento { get; set; }

    /// <summary>Código único de paciente en la base de datos.</summary>
    public string CodigoPaciente { get; set; } = string.Empty;

    /// <summary>Correo electrónico de contacto.</summary>
    public string? CorreoElectronico { get; set; }

    /// <summary>Teléfono de contacto.</summary>
    public string? Telefono { get; set; }

    /// <summary>Historial clínico del paciente.</summary>
    public List<HistorialMedico> Historiales { get; set; } = new();
}
