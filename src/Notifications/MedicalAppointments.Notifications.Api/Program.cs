using System.Reflection;
using System.Text.Json.Serialization;
using MediatR;
using MedicalAppointments.Notifications.Api.Swagger;
using MedicalAppointments.Notifications.Application.Integration;
using MedicalAppointments.Notifications.Application.Queries.GetNotificacionesPaged;
using MedicalAppointments.Notifications.Persistence.Extensions;
using MedicalAppointments.Notifications.Worker;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);

    c.SchemaFilter<EnumStringSchemaFilter>();
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddHttpClient();
builder.Services.AddNotificationPersistence(builder.Configuration);

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(GetNotificacionesPagedQuery).Assembly));

builder.Services.AddSingleton<INotificacionHistorialReporter, NotificacionHistorialReporter>();

// Registra el Worker Service en segundo plano
builder.Services.AddHostedService<NotificationWorker>();

var app = builder.Build();

app.UseCors("AllowAll");

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
