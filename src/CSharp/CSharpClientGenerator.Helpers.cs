using Jeevan.ServiceCraftify.Transformers;
using Jeevan.ServiceCraftify.TypeProcessing;

using Microsoft.OpenApi.Models;

namespace Jeevan.ServiceCraftify.CSharp;

public partial class CSharpClientGenerator
{
    private bool? _generateConsolidatedClient;
    private string[]? _serviceNames;

    private bool GetGenerateConsolidatedClient()
    {
        if (_generateConsolidatedClient.HasValue)
            return _generateConsolidatedClient.Value;

        string[] serviceNames = GetServiceNames();

        bool generateConsolidatedClient = Settings.ConsolidatedClient.Generation switch
        {
            ConsolidatedClientGeneration.IfMultipleClients => serviceNames.Length > 1,
            ConsolidatedClientGeneration.Never => false,
            ConsolidatedClientGeneration.Always => true,
            _ => throw new NotSupportedException($"Unrecognized {nameof(ConsolidatedClientGeneration)} value {Settings.ConsolidatedClient.Generation}"),
        };

        _generateConsolidatedClient = generateConsolidatedClient;
        return generateConsolidatedClient;
    }

    private string[] GetServiceNames()
    {
        if (_serviceNames is not null)
            return _serviceNames;

        _serviceNames = OpenApiDoc.Paths.Values
            .Select(path => path.Operations)
            .SelectMany(operationKvp => operationKvp.Values)
            .Select(operation => operation.GetName())
            .Distinct(StringComparer.Ordinal)
            .Order()
            .ToArray();

        return _serviceNames;
    }

    public IEnumerable<(OpenApiSchema Schema, EnumSchemaType SchemaType)> GetEnumModels()
    {
        return OpenApiDoc.Components.Schemas.Values
            .Where(schema => schema.GetSchemaTypeName() == EnumSchemaType.Name)
            .Select(schema => (schema, (EnumSchemaType)schema.GetSchemaType()));
    }

    public IEnumerable<(OpenApiSchema Schema, ObjectSchemaType SchemaType)> GetObjectModels()
    {
        return OpenApiDoc.Components.Schemas.Values
            .Where(schema => schema.GetSchemaTypeName() == ObjectSchemaType.Name)
            .Select(schema => (schema, (ObjectSchemaType)schema.GetSchemaType()));
    }
    private string TransformConsolidatedClientProperty(string propertyName)
    {
        if (Settings.ConsolidatedClient.PropertyNameTransforms is null or { Length: 0 })
            return propertyName;
        string transformedPropertyName = propertyName;
        foreach (NameTransformer transformer in Settings.ConsolidatedClient.PropertyNameTransforms)
            transformedPropertyName = transformer(transformedPropertyName);
        return transformedPropertyName;
    }
}
