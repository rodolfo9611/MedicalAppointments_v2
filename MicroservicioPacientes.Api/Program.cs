using Microsoft.EntityFrameworkCore;
using MicroservicioPacientes.Application;
using MicroservicioPacientes.Infrastructure;
using MicroservicioPacientes.Domain;

var builder = WebApplication.CreateBuilder(args);

// Configuración de la base de datos en memoria
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("PacientesDb"));

// Inyección de dependencias requerida por la evaluación (Repositorio Inyectado)
builder.Services.AddScoped<IPacienteRepository, PacienteRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Carga de datos iniciales
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    if (!context.Pacientes.Any())
    {
        context.Pacientes.Add(new Paciente
        {
            Nombre = "Carlos",
            Apellido = "Pérez",
            DocumentoIdentidad = "12345678-9",
            FechaNacimiento = new DateTime(1990, 5, 15),
            Historiales = new List<HistorialMedico>
            {
                new HistorialMedico
                {
                    FechaRegistro = DateTime.Now,
                    Diagnostico = "Consulta General",
                    Tratamiento = "Paracetamol 500mg cada 8 horas"
                }
            }
        });
        context.SaveChanges();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();