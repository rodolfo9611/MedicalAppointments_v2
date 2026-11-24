using System.Text.Json.Serialization;
using MediatR;
using MedicalAppointments.Api.Swagger;
using MedicalAppointments.Core.Citas.Queries.GetCitaById;
using MedicalAppointments.Domain.Citas.Interfaces;
using MedicalAppointments.Infrastructure.Integration;
using MedicalAppointments.Persistence.Citas.Context;
using MedicalAppointments.Persistence.Extensions;
using Nuget.Persistence.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
    c.UseInlineDefinitionsForEnums();
    c.SchemaFilter<EnumDescriptionSchemaFilter>();
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

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
builder.Services.AddScoped<IHistorialClinicoClient, HistorialClinicoClient>();

builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(GetCitaByIdQuery).Assembly));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CitasDbContext>();
    await dbContext.EnsureModelTablesAsync();
    await dbContext.MigrarCatalogoEstadosOficialesAsync();
}

app.UseCors("AllowAll");
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
