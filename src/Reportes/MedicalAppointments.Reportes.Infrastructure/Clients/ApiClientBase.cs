using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Nuget.Persistence.Common;

namespace MedicalAppointments.Reportes.Infrastructure.Clients;

/// <summary>
/// Funcionalidad común para los clientes HTTP tipados de solo lectura.
/// </summary>
public abstract class ApiClientBase
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    /// <summary>Construye una cadena de consulta omitiendo parámetros vacíos.</summary>
    protected static string BuildQuery(params (string Key, string? Value)[] parameters)
    {
        var builder = new StringBuilder();

        foreach (var (key, value) in parameters)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                continue;
            }

            builder.Append(builder.Length == 0 ? '?' : '&');
            builder.Append(Uri.EscapeDataString(key))
                   .Append('=')
                   .Append(Uri.EscapeDataString(value));
        }

        return builder.ToString();
    }

    /// <summary>Formatea fechas en un literal ISO independiente de la cultura.</summary>
    protected static string FormatDate(DateTime value) =>
        value.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);

    /// <summary>
    /// Lee una respuesta paginada aceptando tanto <c>PagedDto</c> directo como
    /// <c>HttpResponse&lt;PagedDto&lt;T&gt;&gt;</c>, que es el formato de varios servicios.
    /// </summary>
    protected static async Task<PagedDto<List<T>>?> ReadPagedAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
        where T : class
    {
        response.EnsureSuccessStatusCode();

        if (response.Content.Headers.ContentLength == 0)
        {
            return null;
        }

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

        try
        {
            using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
            var root = document.RootElement;

            if (root.ValueKind == JsonValueKind.Object &&
                TryGetPropertyIgnoreCase(root, "success", out var success) &&
                success.ValueKind == JsonValueKind.False)
            {
                var message = TryGetPropertyIgnoreCase(root, "message", out var messageElement)
                    ? messageElement.GetString()
                    : "El servicio remoto devolvió una respuesta sin éxito.";

                throw new HttpRequestException(message ?? "El servicio remoto devolvió una respuesta sin éxito.");
            }

            var target = root;
            if (root.ValueKind == JsonValueKind.Object)
            {
                if (TryGetPropertyIgnoreCase(root, "data", out var data) && data.ValueKind == JsonValueKind.Object)
                {
                    target = data;
                }
                else if (TryGetPropertyIgnoreCase(root, "result", out var result) && result.ValueKind == JsonValueKind.Object)
                {
                    target = result;
                }
            }

            return target.Deserialize<PagedDto<List<T>>>(JsonOptions);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    /// <summary>Pagina en memoria una colección ya filtrada.</summary>
    protected static PagedDto<List<T>> Paginate<T>(IReadOnlyList<T> source, int pageNumber, int pageSize)
        where T : class
    {
        var total = source.Count;
        var safePage = pageNumber < 1 ? 1 : pageNumber;
        var safeSize = pageSize < 1 ? 10 : pageSize;
        var data = source.Skip((safePage - 1) * safeSize).Take(safeSize).ToList();

        return new PagedDto<List<T>>(total, safePage, safeSize, data);
    }

    /// <summary>Busca una propiedad JSON ignorando mayúsculas y minúsculas.</summary>
    protected static bool TryGetPropertyIgnoreCase(JsonElement element, string name, out JsonElement value)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in element.EnumerateObject())
            {
                if (string.Equals(property.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    value = property.Value;
                    return true;
                }
            }
        }

        value = default;
        return false;
    }
}
