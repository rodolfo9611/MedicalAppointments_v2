using System.ComponentModel;
using System.Reflection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MedicalAppointments.Api.Swagger;

/// <summary>
/// Documenta los valores de los enums con su descripción legible en Swagger UI.
/// </summary>
public class EnumDescriptionSchemaFilter : ISchemaFilter
{
    public void Apply(Microsoft.OpenApi.IOpenApiSchema schema, SchemaFilterContext context)
    {
        var tipo = context.Type.IsEnum ? context.Type : Nullable.GetUnderlyingType(context.Type);
        if (tipo is null || !tipo.IsEnum)
            return;

        var descripciones = tipo.GetFields(BindingFlags.Public | BindingFlags.Static)
            .Select(campo => new
            {
                Nombre = campo.Name,
                Descripcion = campo.GetCustomAttribute<DescriptionAttribute>()?.Description ?? campo.Name
            })
            .ToList();

        if (schema is Microsoft.OpenApi.OpenApiSchema openApiSchema)
        {
            openApiSchema.Description = string.Join(" | ", descripciones.Select(d => $"{d.Nombre}: {d.Descripcion}"));
        }
    }
}
