using MedicalAppointments.Reportes.Domain.Common;
using MedicalAppointments.Reportes.Domain.Interfaces;
using MedicalAppointments.Reportes.Infrastructure.Clients;
using Microsoft.Extensions.DependencyInjection;

namespace MedicalAppointments.Reportes.Infrastructure.Extensions;

/// <summary>
/// Registro de los clientes HTTP tipados utilizados por el microservicio de reportes.
/// </summary>
public static class ReportesHttpClientExtensions
{
    /// <summary>Registra los cuatro clientes HTTP tipados con sus URLs base configuradas.</summary>
    public static IServiceCollection AddReportesApiClients(this IServiceCollection services, ApiUrlsOptions urls)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(urls);

        services.AddHttpClient<IPacientesApiClient, PacientesApiClient>(client =>
            client.BaseAddress = CreateBaseAddress(urls.Pacientes, "Pacientes"));

        services.AddHttpClient<ICitasApiClient, CitasApiClient>(client =>
            client.BaseAddress = CreateBaseAddress(urls.Citas, "Citas"));

        services.AddHttpClient<IRecetasApiClient, RecetasApiClient>(client =>
            client.BaseAddress = CreateBaseAddress(urls.Recetas, "Recetas"));

        services.AddHttpClient<INotificacionesApiClient, NotificacionesApiClient>(client =>
            client.BaseAddress = CreateBaseAddress(urls.Notificaciones, "Notificaciones"));

        return services;
    }

    private static Uri CreateBaseAddress(string url, string servicio)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new InvalidOperationException($"La URL configurada para {servicio} no es válida: '{url}'.");
        }

        return uri;
    }
}
