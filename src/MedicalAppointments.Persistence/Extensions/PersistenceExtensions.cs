using MedicalAppointments.Domain.Citas.Interfaces;
using MedicalAppointments.Persistence.Citas.Context;
using MedicalAppointments.Persistence.Citas.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MedicalAppointments.Persistence.Extensions;

public static class PersistenceExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CitasDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("CitasDB")));

        services.AddScoped<ICitaRepository, CitaRepository>();
        services.AddScoped<IEstadoCitaRepository, EstadoCitaRepository>();

        return services;
    }
}
