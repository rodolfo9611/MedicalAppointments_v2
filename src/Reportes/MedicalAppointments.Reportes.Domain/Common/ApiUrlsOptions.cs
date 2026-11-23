namespace MedicalAppointments.Reportes.Domain.Common;

/// <summary>
/// Configuración de las URLs base de los microservicios que consume el microservicio de reportes.
/// </summary>
public sealed class ApiUrlsOptions
{
    /// <summary>Nombre de la sección de configuración.</summary>
    public const string SectionName = "ApiUrls";

    /// <summary>URL base del microservicio de pacientes.</summary>
    public string Pacientes { get; set; } = "http://localhost:5015";

    /// <summary>URL base del microservicio de citas.</summary>
    public string Citas { get; set; } = "http://localhost:5000";

    /// <summary>URL base del microservicio de recetas médicas.</summary>
    public string Recetas { get; set; } = "http://localhost:5200";

    /// <summary>URL base del microservicio de notificaciones.</summary>
    public string Notificaciones { get; set; } = "http://localhost:5100";
}
