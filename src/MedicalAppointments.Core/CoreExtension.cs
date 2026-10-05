using MedicalAppointments.Core.Citas.Configs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MedicalAppointments.Core;

public static class CoreExtension
{
    /// <summary>
    /// Registra MediatR y enlaza las secciones de appsettings.json con IOptions&lt;T&gt;.
    /// No conoce la librería HTTP: solo opciones y contratos de negocio.
    /// </summary>
    public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CoreExtension).Assembly));

        services.Configure<PacientesOptions>(configuration.GetSection("Pacientes"));

        return services;
    }
}
