using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
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
    /// Inicializa la base de datos del contexto según el proveedor configurado.
    /// </summary>
    public static async Task InitializeConfiguredDatabaseAsync<TContext>(
        this IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
        where TContext : DbContext
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TContext>();
        await context.EnsureModelTablesAsync(cancellationToken);
    }

    /// <summary>
    /// Asegura las tablas del modelo en el proveedor configurado. Con una única BD
    /// CitasDB, EnsureCreated no basta: si la BD ya existe con tablas de otro
    /// microservicio, no crea las tablas que faltan de este DbContext.
    /// </summary>
    public static async Task EnsureModelTablesAsync(
        this DbContext context,
        CancellationToken cancellationToken = default)
    {
        await context.Database.EnsureCreatedAsync(cancellationToken);

        if (!context.Database.IsRelational())
            return;

        var tablasModelo = context.Model.GetEntityTypes()
            .Select(entity => entity.GetTableName())
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (tablasModelo.Length == 0)
            return;

        var existentes = await ObtenerTablasExistentesAsync(context, tablasModelo, cancellationToken);
        if (existentes.Count == tablasModelo.Length)
            return;

        if (existentes.Count > 0)
        {
            var faltan = tablasModelo.Where(table => !existentes.Contains(table));
            throw new InvalidOperationException(
                $"El modelo {context.GetType().Name} tiene tablas parciales en CitasDB. " +
                $"Faltan: {string.Join(", ", faltan)}.");
        }

        var creator = context.GetService<IRelationalDatabaseCreator>();
        await creator.CreateTablesAsync(cancellationToken);
    }

    private static async Task<HashSet<string>> ObtenerTablasExistentesAsync(
        DbContext context,
        IReadOnlyCollection<string> tablas,
        CancellationToken cancellationToken)
    {
        var resultado = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var conexion = context.Database.GetDbConnection();
        await using var comando = conexion.CreateCommand();

        var parametros = tablas.Select((tabla, indice) => ($"@tabla{indice}", tabla)).ToArray();
        comando.CommandText = $"""
            SELECT TABLE_NAME
            FROM INFORMATION_SCHEMA.TABLES
            WHERE TABLE_TYPE = 'BASE TABLE'
              AND TABLE_NAME IN ({string.Join(", ", parametros.Select(p => p.Item1))})
            """;

        foreach (var (nombre, valor) in parametros)
        {
            var parametro = comando.CreateParameter();
            parametro.ParameterName = nombre;
            parametro.Value = valor;
            comando.Parameters.Add(parametro);
        }

        var abrirConexion = conexion.State != ConnectionState.Open;
        if (abrirConexion)
            await conexion.OpenAsync(cancellationToken);

        try
        {
            await using var lector = await comando.ExecuteReaderAsync(cancellationToken);
            while (await lector.ReadAsync(cancellationToken))
                resultado.Add(lector.GetString(0));
        }
        finally
        {
            if (abrirConexion)
                await conexion.CloseAsync();
        }

        return resultado;
    }
}
