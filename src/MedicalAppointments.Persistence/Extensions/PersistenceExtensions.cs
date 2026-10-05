using MedicalAppointments.Core.Citas.Services;
using MedicalAppointments.Domain.Citas.Interfaces;
using MedicalAppointments.Persistence.Citas.Context;
using MedicalAppointments.Persistence.Citas.Repositories;
using MedicalAppointments.Persistence.Citas.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NugetPackage_Rest.Builders;
using NugetPackage_Rest.Extensions;
using NugetPackage_Rest.Interfaces.IServices;

namespace MedicalAppointments.Persistence.Extensions;

public static class PersistenceExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CitasDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("CitasDB")));

        services.AddScoped<ICitaRepository, CitaRepository>();
        services.AddScoped<IEstadoCitaRepository, EstadoCitaRepository>();

        // 1. Enlaza "RestSettings" -> IOptions<RequestSettings> que recibe RestBuilder
        services.AddRequestLogging(configuration);

        // 2. Registra IRest como typed client de IHttpClientFactory (pool de handlers, sin agotar sockets)
        services.AddHttpClient<IRest, RestBuilder>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
            client.DefaultRequestHeaders.Add("X-Origen", "Citas");
        });

        // 3. Registra los servicios que consumen el otro microservicio a través de IRest
        services.AddScoped<IPacienteService, PacienteService>();

        return services;
    }
}
