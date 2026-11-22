using MedicalAppointments.Notifications.Domain.Interfaces;
using MedicalAppointments.Notifications.Persistence.Context;
using MedicalAppointments.Notifications.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MedicalAppointments.Notifications.Persistence.Extensions;

public static class NotificationsPersistenceExtensions
{
    public static IServiceCollection AddNotificationPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("NotificationsDB") 
                            ?? configuration.GetConnectionString("CitasDB");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = "Server=DESKTOP-B9GUC6S\\SQLEXPRESS02;Database=CitasDB;Trusted_Connection=True;TrustServerCertificate=True;";
        }

        services.AddDbContext<NotificationsDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<INotificacionRepository, NotificacionRepository>();

        return services;
    }
}
