using System.Text.Json.Serialization;
using MediatR;
using MedicalAppointments.RecetasMedicas.Api.Integration;
using MedicalAppointments.RecetasMedicas.Application.Queries.GetRecetaById;
using MedicalAppointments.RecetasMedicas.Domain.Interfaces;
using MedicalAppointments.RecetasMedicas.Persistence.Context;
using MedicalAppointments.RecetasMedicas.Persistence.Extensions;
using Nuget.Persistence.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
    }
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

var apiUrls = builder.Configuration.GetSection("ApiUrls");
builder.Services.AddHttpClient<ICitasApiClient, CitasApiClient>(client =>
    client.BaseAddress = new Uri(apiUrls["Citas"] ?? "http://localhost:5000"));

builder.Services.AddHttpClient<IPacientesApiClient, PacientesApiClient>(client =>
    client.BaseAddress = new Uri(apiUrls["Pacientes"] ?? "http://localhost:5015"));

builder.Services.AddHttpClient<INotificacionesApiClient, NotificacionesApiClient>(client =>
    client.BaseAddress = new Uri(apiUrls["Notificaciones"] ?? "http://localhost:5100"));

builder.Services.AddRecetasPersistence(builder.Configuration);
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(GetRecetaByIdQuery).Assembly));

var app = builder.Build();

await app.Services.InitializeConfiguredDatabaseAsync<RecetasDbContext>();

app.UseCors("AllowAll");
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();
