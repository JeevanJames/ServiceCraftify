using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;

namespace Jeevan.ServiceCraftify;

public abstract class DocumentProcessor(OpenApiDocument document)
{
    public OpenApiDocument Document { get; } = document ?? throw new ArgumentNullException(nameof(document));

    public abstract void Process();
}

public abstract class DocumentProcessor<TSettings>(OpenApiDocument document, TSettings settings)
    : DocumentProcessor(document)
    where TSettings : GeneratorSettings
{
    public TSettings Settings { get; } = settings ?? throw new ArgumentNullException(nameof(settings));
}

public class TransformDocumentProcessor(OpenApiDocument document, GeneratorSettings settings)
    : DocumentProcessor<GeneratorSettings>(document, settings)
{
    public override void Process()
    {
        foreach (KeyValuePair<string, OpenApiPathItem> pathKvp in Document.Paths)
        {
            foreach (OpenApiOperation operation in pathKvp.Value.Operations.Values)
            {
                string originalServiceName = operation.Tags.FirstOrDefault()?.Name ?? Settings.DefaultServiceName;
                string transformedServiceName = originalServiceName;
                if (Settings.Transformers.HasServiceNameTransformers)
                {
                    foreach (NameTransformer transformer in Settings.Transformers.ServiceNames)
                        transformedServiceName = transformer(transformedServiceName);
                }

                operation.SetName(transformedServiceName);

                string transformedOperationName = operation.OperationId;
                if (Settings.Transformers.HasOperationNameTransformers)
                {
                    foreach (ChildNameTransformer transformer in Settings.Transformers.OperationNames)
                        transformedOperationName = transformer(transformedOperationName, transformedServiceName, originalServiceName);
                }

                operation.SetName(transformedOperationName, operation.OperationId);
            }
        }

        foreach (KeyValuePair<string, OpenApiSchema> modelSchemaKvp in Document.Components.Schemas)
        {
            (string modelName, OpenApiSchema modelSchema) = modelSchemaKvp;

            string transformedModelName = modelName;
            if (Settings.Transformers.HasModelNameTransformers)
            {
                foreach (NameTransformer transformer in Settings.Transformers.ModelNames)
                    transformedModelName = transformer(transformedModelName);
                modelSchemaKvp.SetName(transformedModelName);
            }

            foreach (KeyValuePair<string, OpenApiSchema> propertySchemaKvp in modelSchema.Properties)
            {
                string propertyName = propertySchemaKvp.Key;

                if (Settings.Transformers.HasPropertyNameTransformers)
                {
                    string transformedName = propertyName;
                    foreach (ChildNameTransformer transformer in Settings.Transformers.PropertyNames)
                        transformedName = transformer(transformedName, transformedModelName, modelName);
                    propertySchemaKvp.SetName(transformedName, propertyName);
                }
            }
        }
    }
}

#pragma warning disable S4136 // Method overloads should be grouped together
public static class ExtensionExtensions
{
    //public static string GetName(this KeyValuePair<string, OpenApiSchema> schemaKvp, string? propertyName = null)
    //{
    //    string extensionKey = propertyName is null ? ExtensionKey.Name : $"{ExtensionKey.Name}_[{propertyName}]";
    //    return schemaKvp.Value.Extensions.TryGetValue(extensionKey, out IOpenApiExtension? nameEntry) &&
    //        nameEntry is OpenApiString name ? name.Value : schemaKvp.Key;
    //}

    //internal static void SetName(this KeyValuePair<string, OpenApiSchema> schemaKvp, string value, string? propertyName = null)
    //{
    //    string extensionsKey = propertyName is null ? ExtensionKey.Name : $"{ExtensionKey.Name}_[{propertyName}]";
    //    schemaKvp.Value.Extensions[extensionsKey] = new OpenApiString(value);
    //}

    public static string GetName<TExtensible>(this KeyValuePair<string, TExtensible> extensibleKvp, string? childName = null)
        where TExtensible : IOpenApiExtensible
    {
        string extensionKey = childName is null ? ExtensionKey.Name : $"{ExtensionKey.Name}_[{childName}]";
        return extensibleKvp.Value.Extensions.TryGetValue(extensionKey, out IOpenApiExtension? nameEntry) &&
            nameEntry is OpenApiString name ? name.Value : extensibleKvp.Key;
    }

    internal static void SetName<TExtensible>(this KeyValuePair<string, TExtensible> extensibleKvp, string value, string? childName = null)
        where TExtensible : IOpenApiExtensible
    {
        string extensionsKey = childName is null ? ExtensionKey.Name : $"{ExtensionKey.Name}_[{childName}]";
        extensibleKvp.Value.Extensions[extensionsKey] = new OpenApiString(value);
    }

    public static string GetName(this IOpenApiExtensible extensible, string? childName = null)
    {
        string extensionKey = childName is null ? ExtensionKey.Name : $"{ExtensionKey.Name}_[{childName}]";
        if (!extensible.Extensions.TryGetValue(extensionKey, out IOpenApiExtension? entry))
            throw new InvalidOperationException($"No entry for {extensionKey}.");
        if (entry is not OpenApiString name)
            throw new InvalidOperationException($"Entry {extensionKey} with value {entry} is not a string.");
        return name.Value;
    }

    internal static void SetName(this IOpenApiExtensible extensible, string value, string? childName = null)
    {
        string extensionKey = childName is null ? ExtensionKey.Name : $"{ExtensionKey.Name}_[{childName}]";
        extensible.Extensions[extensionKey] = new OpenApiString(value);
    }
}

public static class ExtensionKey
{
    public const string Name = "@@Name";
}
