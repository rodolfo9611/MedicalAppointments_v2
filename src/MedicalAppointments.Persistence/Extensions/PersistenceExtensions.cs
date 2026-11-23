using MedicalAppointments.Domain.Citas.Interfaces;
using MedicalAppointments.Persistence.Citas.Context;
using MedicalAppointments.Persistence.Citas.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nuget.Persistence.Extensions;

namespace MedicalAppointments.Persistence.Extensions;

public static class PersistenceExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddConfiguredDbContext<CitasDbContext>(
            configuration,
            connectionStringName: "CitasDB",
            inMemoryDatabaseName: "CitasDb");

        services.AddScoped<ICitaRepository, CitaRepository>();
        services.AddScoped<IEstadoCitaRepository, EstadoCitaRepository>();

        return services;
    }
}
