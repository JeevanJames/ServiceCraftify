using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;

namespace Jeevan.ServiceCraftify.TypeProcessing;

public sealed class TypeDocumentProcessor(OpenApiDocument document) : DocumentProcessor(document)
{
    public override void Process()
    {
        foreach ((_, OpenApiSchema schema) in Document.Components.Schemas)
        {
            if (schema.Enum is { Count: > 0 })
                ProcessAsEnum(schema);
            else
                ProcessAsObject(schema);
        }
    }

    private static void ProcessAsEnum(OpenApiSchema schema)
    {
        EnumSchemaType type = new();

        type.Details.UnderlyingType = schema.Type switch
        {
            "string" => EnumUnderlyingType.String,
            "integer" => EnumUnderlyingType.Integer,
            "number" => EnumUnderlyingType.Number,
            _ => throw new NotSupportedException($"Unrecognized enum type {schema.Type}."),
        };

        // Get the enum member names and values.
        List<EnumMember> enumMembers;
        if (type.Details.UnderlyingType == EnumUnderlyingType.String)
        {
            enumMembers = schema.Enum.Cast<OpenApiString>()
                .Select((oas, index) => new EnumMember(oas.Value, index))
                .ToList();
        }
        else
        {
            // Check if name overrides are specified using the x-enumNames custom extension.
            string[]? enumOverrideNames = null;
            if (schema.Extensions.TryGetValue("x-enumNames", out IOpenApiExtension? extNames) && extNames is OpenApiArray enumNamesArray)
                enumOverrideNames = enumNamesArray.OfType<OpenApiString>().Select(oas => oas.Value).ToArray();

            // Get the integer values of the enum members.
            int[] intEnums = schema.Enum.Cast<OpenApiInteger>().Select(oai => oai.Value).ToArray();

            // If name overrides are specified, their count must match the number of enum members.
            if (enumOverrideNames is not null && intEnums.Length != enumOverrideNames.Length)
                throw new CraftifyException("Enum member count does not match enum names count.");

            if (enumOverrideNames is null)
                enumMembers = intEnums.Select(ie => new EnumMember($"_{ie}", ie)).ToList();
            else
                enumMembers = enumOverrideNames.Zip(intEnums, (name, value) => new EnumMember(name, value)).ToList();
        }

        // Get descriptions if available.
        if (schema.Extensions.TryGetValue("x-enumDescriptions", out IOpenApiExtension? extDesc) && extDesc is OpenApiArray enumDescArray)
        {
            string[] enumDescriptions = enumDescArray.Cast<OpenApiString>().Select(oas => oas.Value).ToArray();

            if (enumMembers.Count != enumDescriptions.Length)
                throw new CraftifyException("Enum member count does not match enum descriptions count.");

            for (int i = 0; i < enumMembers.Count; i++)
                enumMembers[i].Description = enumDescriptions[i];
        }

        foreach (EnumMember enumMember in enumMembers)
            type.Details.Members.Add(enumMember);

        schema.SetSchemaType(EnumSchemaType.Name, type);
    }

    private static void ProcessAsObject(OpenApiSchema schema)
    {
        schema.SetSchemaType("object", new ObjectSchemaType());
    }
}
