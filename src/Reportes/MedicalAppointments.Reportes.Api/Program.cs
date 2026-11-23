using System.Reflection;
using System.Text.Json.Serialization;
using MediatR;
using MedicalAppointments.Reportes.Application.Queries.Dashboard;
using MedicalAppointments.Reportes.Domain.Common;
using MedicalAppointments.Reportes.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var assemblies = new[]
    {
        typeof(GetDashboardQuery).Assembly,
        typeof(ApiUrlsOptions).Assembly,
        typeof(ReportesHttpClientExtensions).Assembly,
        Assembly.GetExecutingAssembly()
    };

    foreach (var assembly in assemblies.Distinct())
    {
        var xmlFile = $"{assembly.GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

        if (File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath);
        }
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

var apiUrls = builder.Configuration.GetSection(ApiUrlsOptions.SectionName).Get<ApiUrlsOptions>() ?? new ApiUrlsOptions();
builder.Services.AddReportesApiClients(apiUrls);

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(GetDashboardQuery).Assembly));

var app = builder.Build();

app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
