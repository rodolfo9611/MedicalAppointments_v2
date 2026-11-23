using MedicalAppointments.Notifications.Application.Integration;
using MedicalAppointments.Notifications.Persistence.Extensions;
using MedicalAppointments.Notifications.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddNotificationPersistence(builder.Configuration);
builder.Services.AddSingleton<INotificacionHistorialReporter, NotificacionHistorialReporter>();
builder.Services.AddHostedService<NotificationWorker>();

var host = builder.Build();
host.Run();
