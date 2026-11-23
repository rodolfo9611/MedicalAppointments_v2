using MedicalAppointments.RecetasMedicas.Domain.Interfaces;
using MedicalAppointments.RecetasMedicas.Persistence.Context;
using MedicalAppointments.RecetasMedicas.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nuget.Persistence.Extensions;

namespace MedicalAppointments.RecetasMedicas.Persistence.Extensions;

public static class RecetasPersistenceExtensions
{
    /// <summary>
    /// Registra RecetasDbContext según Persistence:Provider (InMemory | SqlServer)
    /// y el repositorio concreto de recetas.
    /// </summary>
    public static IServiceCollection AddRecetasPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddConfiguredDbContext<RecetasDbContext>(
            configuration,
            "DefaultConnection",
            "RecetasDb");

        services.AddScoped<IRecetaMedicaRepository, RecetaMedicaRepository>();
        return services;
    }
}
