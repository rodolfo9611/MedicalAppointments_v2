using System.Text.Json.Serialization;
using MediatR;
using MicroservicioPacientes.Application;
using MicroservicioPacientes.Application.Queries.GetPacientes;
using MicroservicioPacientes.Domain;
using MicroservicioPacientes.Infrastructure;
using Nuget.Persistence.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Persistencia configurable por configuración: InMemory | SqlServer.
builder.Services.AddConfiguredDbContext<ApplicationDbContext>(
    builder.Configuration,
    "DefaultConnection",
    "PacientesDb");

builder.Services.AddScoped<IPacienteRepository, PacienteRepository>();
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(GetPacientesQuery).Assembly));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
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

var app = builder.Build();

// Si se usa SQL Server se crea la base de datos de forma segura; en InMemory no aplica.
if (string.Equals(builder.Configuration["Persistence:Provider"], "SqlServer", StringComparison.OrdinalIgnoreCase))
{
    try
    {
        await app.Services.InitializeConfiguredDatabaseAsync<ApplicationDbContext>();
    }
    catch (Exception ex)
    {
        app.Logger.LogWarning(ex, "No se pudo inicializar la base de datos de Pacientes.");
    }
}

// Carga de datos inicial: catálogo clínico ficticio con nombres reales, sin registros de prueba.
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    var pacientesIniciales = new[]
    {
        new PacienteSemilla("Carlos", "Pérez", "06123456-7", new DateTime(1988, 4, 12), "PAC-000001", "carlos.perez@correo-clinica.com", "+503 7000 1234", "Consulta general", "Consulta general de control inicial.", "Paracetamol 500 mg cada 8 horas", "ConsultaGeneral"),
        new PacienteSemilla("Lucía", "Martínez", "06123457-5", new DateTime(1992, 8, 23), "PAC-000002", "lucia.martinez@correo-clinica.com", "+503 7123 4567", "Hipertensión arterial", "Control de presión arterial.", "Losartán 50 mg diario", "Control"),
        new PacienteSemilla("Diego", "Ramírez", "06123458-3", new DateTime(1979, 11, 4), "PAC-000003", "diego.ramirez@correo-clinica.com", "+503 7234 5678", "Evaluación cardiológica", "Evaluación cardiológica de seguimiento.", "Ecoterapia y control periódico", "ConsultaGeneral"),
        new PacienteSemilla("Sofía", "Hernández", "06123459-1", new DateTime(1985, 2, 17), "PAC-000004", "sofia.hernandez@correo-clinica.com", "+503 7345 6789", "Revisión dermatológica", "Revisión dermatológica.", "Protector solar y crema tópica", "Control"),
        new PacienteSemilla("Andrés", "López", "06123460-9", new DateTime(1996, 6, 30), "PAC-000005", "andres.lopez@correo-clinica.com", "+503 7456 7890", "Malestar digestivo", "Consulta general por malestar estomacal.", "Dieta blanda e hidratación", "ConsultaGeneral"),
        new PacienteSemilla("Valentina", "Cruz", "06123461-7", new DateTime(2001, 9, 12), "PAC-000006", "valentina.cruz@correo-clinica.com", "+503 7567 8901", "Control pediátrico", "Control pediátrico de crecimiento.", "Suplemento vitamínico", "Control"),
        new PacienteSemilla("Mauricio", "Ávalos", "06123462-5", new DateTime(1972, 3, 8), "PAC-000007", "mauricio.avalos@correo-clinica.com", "+503 7678 9012", "Lumbalgia", "Dolor lumbar persistente.", "Fisioterapia y analgésico", "ConsultaGeneral"),
        new PacienteSemilla("Patricia", "Solís", "06123463-3", new DateTime(1983, 7, 21), "PAC-000008", "patricia.solis@correo-clinica.com", "+503 7789 0123", "Control ginecológico", "Control ginecológico anual.", "Examen de control y orientación", "Control"),
        new PacienteSemilla("Javier", "Santana", "06123464-1", new DateTime(1990, 12, 5), "PAC-000009", "javier.santana@correo-clinica.com", "+503 7890 1234", "Revisión visual", "Revisión de visión.", "Uso de lentes correctivos", "ConsultaGeneral"),
        new PacienteSemilla("Elena", "Rosales", "06123465-9", new DateTime(1968, 10, 14), "PAC-000010", "elena.rosales@correo-clinica.com", "+503 7901 2345", "Seguimiento crónico", "Seguimiento de tratamiento crónico.", "Control mensual de laboratorio", "Control")
    };

    for (var i = 0; i < pacientesIniciales.Length; i++)
    {
        var semilla = pacientesIniciales[i];
        var paciente = context.Pacientes.FirstOrDefault(p =>
            p.CodigoPaciente == semilla.Codigo || p.DocumentoIdentidad == semilla.Documento);

        if (paciente is null)
        {
            paciente = new Paciente
            {
                Nombre = semilla.Nombre,
                Apellido = semilla.Apellido,
                DocumentoIdentidad = semilla.Documento,
                FechaNacimiento = semilla.FechaNacimiento,
                CodigoPaciente = semilla.Codigo,
                CorreoElectronico = semilla.Correo,
                Telefono = semilla.Telefono
            };
            context.Pacientes.Add(paciente);
        }
        else
        {
            // Compatibilidad con bases ya creadas: se actualizan los datos maestros.
            paciente.Nombre = semilla.Nombre;
            paciente.Apellido = semilla.Apellido;
            paciente.DocumentoIdentidad = semilla.Documento;
            paciente.FechaNacimiento = semilla.FechaNacimiento;
            paciente.CodigoPaciente = semilla.Codigo;
            paciente.CorreoElectronico = semilla.Correo;
            paciente.Telefono = semilla.Telefono;
        }

        var eventKey = $"SEED-{semilla.Codigo}-001";
        if (!context.HistorialesMedicos.Any(h => h.EventKey == eventKey))
        {
            paciente.Historiales.Add(new HistorialMedico
            {
                FechaRegistro = DateTime.Now,
                FechaEvento = DateTime.Today.AddDays(-45 + i * 4),
                Diagnostico = semilla.Diagnostico,
                Tratamiento = semilla.Tratamiento,
                TipoEvento = semilla.TipoEvento,
                Descripcion = semilla.Descripcion,
                EventKey = eventKey
            });
        }
    }

    context.SaveChanges();
}

app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();

internal sealed record PacienteSemilla(
    string Nombre,
    string Apellido,
    string Documento,
    DateTime FechaNacimiento,
    string Codigo,
    string Correo,
    string Telefono,
    string Diagnostico,
    string Descripcion,
    string Tratamiento,
    string TipoEvento);
