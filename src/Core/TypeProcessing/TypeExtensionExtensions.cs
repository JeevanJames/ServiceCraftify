using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace Jeevan.ServiceCraftify.TypeProcessing;

public static class TypeExtensionExtensions
{
    private const string TypeKey = "@@Type";
    private const string SchemaTypeNameKey = "@@SchemaTypeName";

    public static SchemaType GetSchemaType(this OpenApiSchema schema) =>
        SchemaType.CreateSchemaTypeInstance(schema.GetSchemaTypeName(), schema.GetExtensionValue(TypeKey));

    public static string GetSchemaTypeName(this OpenApiSchema schema) =>
        schema.GetExtensionValue(SchemaTypeNameKey);

    internal static void SetSchemaType(this OpenApiSchema schema, string schemaTypeName, SchemaType schemaType)
    {
        schema.Extensions[SchemaTypeNameKey] = new OpenApiString(schemaTypeName);
        schema.Extensions[TypeKey] = new OpenApiString(schemaType.Encode());
    }
}

