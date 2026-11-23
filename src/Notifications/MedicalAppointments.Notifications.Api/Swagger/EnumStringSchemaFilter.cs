using System.Text.Json.Nodes;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MedicalAppointments.Notifications.Api.Swagger;

/// <summary>
/// Documenta los enums como cadenas, coherente con JsonStringEnumConverter.
/// </summary>
public sealed class EnumStringSchemaFilter : ISchemaFilter
{
    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {
        var enumType = Nullable.GetUnderlyingType(context.Type) ?? context.Type;
        if (!enumType.IsEnum || schema is not OpenApiSchema openApiSchema)
            return;

        openApiSchema.Type = JsonSchemaType.String;
        openApiSchema.Format = null;
        openApiSchema.Enum = Enum.GetNames(enumType)
            .Select(nombre => (JsonNode)JsonValue.Create(nombre)!)
            .ToList();
    }
}
