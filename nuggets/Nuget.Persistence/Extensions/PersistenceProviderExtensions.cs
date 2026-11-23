using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Nuget.Persistence.Extensions;

/// <summary>
/// Registra un DbContext seleccionando el proveedor de persistencia exclusivamente
/// mediante configuración, sin modificar la lógica de negocio de los microservicios.
/// </summary>
public static class PersistenceProviderExtensions
{
    /// <summary>
    /// Agrega un DbContext usando Persistence:Provider = InMemory | SqlServer.
    /// </summary>
    public static IServiceCollection AddConfiguredDbContext<TContext>(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionStringName = "DefaultConnection",
        string? inMemoryDatabaseName = null)
        where TContext : DbContext
    {
        var provider = configuration["Persistence:Provider"] ?? "SqlServer";
        var databaseName = configuration["Persistence:InMemoryDatabaseName"]
                           ?? inMemoryDatabaseName
                           ?? typeof(TContext).Name;

        if (string.Equals(provider, "InMemory", StringComparison.OrdinalIgnoreCase))
        {
            services.AddDbContext<TContext>(options => options.UseInMemoryDatabase(databaseName));
            return services;
        }

        var connectionString = configuration.GetConnectionString(connectionStringName)
                               ?? configuration.GetConnectionString("CitasDB")
                               ?? configuration.GetConnectionString("NotificationsDB");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"No se encontró la cadena de conexión '{connectionStringName}' para el proveedor SqlServer.");
        }

        services.AddDbContext<TContext>(options => options.UseSqlServer(connectionString));
        return services;
    }

    /// <summary>
    /// Inicializa la base de datos del contexto according to the configured provider.
    /// </summary>
    public static async Task InitializeConfiguredDatabaseAsync<TContext>(
        this IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
        where TContext : DbContext
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TContext>();
        await context.Database.EnsureCreatedAsync(cancellationToken);
    }
}
