namespace MedicalAppointments.Core.Citas.Configs;

/// <summary>
/// URL base del microservicio de Pacientes a el que llama el de Citas.
/// Se lee de la sección "Pacientes" del appsettings.json. Sin barra final.
/// </summary>
public class PacientesOptions
{
    public string BaseUrl { get; set; } = string.Empty;
}
