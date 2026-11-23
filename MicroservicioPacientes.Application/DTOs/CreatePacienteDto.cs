using System.ComponentModel.DataAnnotations;
using MicroservicioPacientes.Application.Validation;

namespace MicroservicioPacientes.Application.DTOs;

/// <summary>
/// Datos necesarios para crear un paciente.
/// </summary>
public class CreatePacienteDto
{
    /// <summary>Nombre del paciente.</summary>
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Apellido del paciente.</summary>
    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [StringLength(100)]
    public string Apellido { get; set; } = string.Empty;

    /// <summary>Documento de identidad del paciente.</summary>
    [Required(ErrorMessage = "El documento de identidad es obligatorio.")]
    [StringLength(30)]
    public string DocumentoIdentidad { get; set; } = string.Empty;

    /// <summary>
    /// Alias compatible con el contrato maestro usado por otros servicios.
    /// </summary>
    public string Documento
    {
        get => DocumentoIdentidad;
        set => DocumentoIdentidad = value;
    }

    /// <summary>Fecha de nacimiento del paciente.</summary>
    [NotFutureDate(ErrorMessage = "La fecha de nacimiento no puede ser futura.")]
    public DateTime FechaNacimiento { get; set; }

    /// <summary>Código único del paciente.</summary>
    [Required(ErrorMessage = "El código de paciente es obligatorio.")]
    [StringLength(30)]
    public string CodigoPaciente { get; set; } = string.Empty;

    /// <summary>Correo electrónico del paciente.</summary>
    [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
    [StringLength(150)]
    public string? CorreoElectronico { get; set; }

    /// <summary>Teléfono del paciente.</summary>
    [Phone(ErrorMessage = "El teléfono no es válido.")]
    [StringLength(30)]
    public string? Telefono { get; set; }
}
