using MedicalAppointments.Notifications.Domain.Interfaces;
using MedicalAppointments.Notifications.Persistence.Context;
using MedicalAppointments.Notifications.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nuget.Persistence.Extensions;

namespace MedicalAppointments.Notifications.Persistence.Extensions;

public static class NotificationsPersistenceExtensions
{
    private const string FallbackConnectionString =
        "Server=DESKTOP-B9GUC6S\\SQLEXPRESS02;Database=CitasDB;Trusted_Connection=True;TrustServerCertificate=True;";

    public static IServiceCollection AddNotificationPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Conserva el fallback histórico cuando no hay configuración SQL Server explícita.
        var provider = configuration["Persistence:Provider"];
        var hasConnectionString =
            !string.IsNullOrWhiteSpace(configuration.GetConnectionString("NotificationsDB")) ||
            !string.IsNullOrWhiteSpace(configuration.GetConnectionString("CitasDB")) ||
            !string.IsNullOrWhiteSpace(configuration.GetConnectionString("DefaultConnection"));

        if (!string.Equals(provider, "InMemory", StringComparison.OrdinalIgnoreCase) && !hasConnectionString)
        {
            configuration = new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:NotificationsDB"] = FallbackConnectionString
                })
                .Build();
        }

        services.AddConfiguredDbContext<NotificationsDbContext>(
            configuration,
            "NotificationsDB",
            "NotificationsDb");

        services.AddScoped<INotificacionRepository, NotificacionRepository>();

        return services;
    }
}
