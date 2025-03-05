using Microsoft.OpenApi.Models;

namespace Jeevan.ServiceCraftify.CSharp.VariableProcessing;

internal static class CSharpTypeHelper
{
    internal static string GetNonObjectType(OpenApiSchema schema)
    {
        Type? type = schema.Type switch
        {
            OpenApiSchemaTypes.String => schema.Format switch
            {
                OpenApiSchemaFormats.Strings.Date => typeof(DateOnly),
                OpenApiSchemaFormats.Strings.DateTime => typeof(DateTimeOffset),
                OpenApiSchemaFormats.Strings.Binary => typeof(byte[]),
                OpenApiSchemaFormats.Strings.Byte => typeof(Guid),
                OpenApiSchemaFormats.Strings.Uri => typeof(Uri),
                _ => typeof(string),
            },
            OpenApiSchemaTypes.Integer => schema.Format switch
            {
                OpenApiSchemaFormats.Integers.Int32 => typeof(int),
                OpenApiSchemaFormats.Integers.Int64 => typeof(long),
                _ => typeof(int),
            },
            OpenApiSchemaTypes.Number => schema.Format switch
            {
                OpenApiSchemaFormats.Numbers.Float => typeof(float),
                OpenApiSchemaFormats.Numbers.Double => typeof(double),
                OpenApiSchemaFormats.Numbers.Decimal => typeof(decimal),
                _ => typeof(double),
            },
            OpenApiSchemaTypes.Boolean => typeof(bool),
            OpenApiSchemaTypes.Object => throw new InvalidOperationException("Should not encounter an object schema type here."),
            _ => null,
        };

        if (type is null)
            return string.Empty;

        string typeName = type.FullName ?? throw new CraftifyException($"Could not get the full type name of {type}.");
        return GetCSharpTypeAlias(typeName);
    }

    internal static string GetCSharpTypeAlias(string dotnetTypeName) =>
        CSharpTypeAliasMapping.TryGetValue(dotnetTypeName, out string? alias) ? alias : dotnetTypeName;

    private static readonly Dictionary<string, string> CSharpTypeAliasMapping = new()
    {
        [typeof(bool).FullName!] = "bool",
        [typeof(byte).FullName!] = "byte",
        [typeof(sbyte).FullName!] = "sbyte",
        [typeof(char).FullName!] = "char",
        [typeof(decimal).FullName!] = "decimal",
        [typeof(double).FullName!] = "double",
        [typeof(float).FullName!] = "float",
        [typeof(int).FullName!] = "int",
        [typeof(uint).FullName!] = "uint",
        [typeof(long).FullName!] = "long",
        [typeof(ulong).FullName!] = "ulong",
        [typeof(object).FullName!] = "object",
        [typeof(short).FullName!] = "short",
        [typeof(ushort).FullName!] = "ushort",
        [typeof(string).FullName!] = "string",
    };
}
