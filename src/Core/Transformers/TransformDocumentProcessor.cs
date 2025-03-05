using Microsoft.OpenApi.Models;

namespace Jeevan.ServiceCraftify.Transformers;

public class TransformDocumentProcessor(OpenApiDocument document, GeneratorSettings settings)
    : DocumentProcessor<GeneratorSettings>(document, settings)
{
    public override void Process()
    {
        // Transform service, operation and parameter names.
        foreach (KeyValuePair<string, OpenApiPathItem> pathKvp in Document.Paths)
        {
            foreach (OpenApiOperation operation in pathKvp.Value.Operations.Values)
            {
                // Get the service name from the first operation tag or the default name if no tags
                // exist, and transform it.
                string originalServiceName = operation.Tags.FirstOrDefault()?.Name ?? Settings.DefaultServiceName;
                string transformedServiceName = originalServiceName;
                if (Settings.Transformers.HasServiceNameTransformers)
                {
                    foreach (NameTransformer transformer in Settings.Transformers.ServiceNames)
                        transformedServiceName = transformer(transformedServiceName);
                }

                operation.SetName(transformedServiceName);

                // Get the operation name from the operations OperationId property and transform it.
                // Since we're setting the operation name of the same OpenApiOperation instance where
                // we set the service name, we need to use a child key.
                string transformedOperationName = operation.OperationId;
                if (Settings.Transformers.HasOperationNameTransformers)
                {
                    foreach (ChildNameTransformer transformer in Settings.Transformers.OperationNames)
                        transformedOperationName = transformer(transformedOperationName, transformedServiceName, originalServiceName);
                }

                operation.SetName(transformedOperationName, operation.OperationId);

                // Get the operation parameter names and transform them.
                foreach (OpenApiParameter parameter in operation.Parameters)
                {
                    string transformedParameterName = parameter.Name;
                    if (Settings.Transformers.HasParameterNameTransformers)
                    {
                        foreach (ParameterNameTransformer transformer in Settings.Transformers.ParameterNames)
                        {
                            transformedParameterName = transformer(
                                transformedParameterName, transformedOperationName, transformedServiceName);
                        }
                    }

                    parameter.SetName(transformedParameterName);
                }
            }
        }

        // Transform schemas models and their properties.
        foreach ((string modelName, OpenApiSchema modelSchema) in Document.Components.Schemas)
        {
            string transformedModelName = modelName;
            if (Settings.Transformers.HasModelNameTransformers)
            {
                foreach (NameTransformer transformer in Settings.Transformers.ModelNames)
                    transformedModelName = transformer(transformedModelName);
            }

            modelSchema.SetName(transformedModelName);

            foreach (KeyValuePair<string, OpenApiSchema> propertySchemaKvp in modelSchema.Properties)
            {
                string propertyName = propertySchemaKvp.Key;

                string transformedPropertyName = propertyName;
                if (Settings.Transformers.HasPropertyNameTransformers)
                {
                    foreach (ChildNameTransformer transformer in Settings.Transformers.PropertyNames)
                        transformedPropertyName = transformer(transformedPropertyName, transformedModelName, modelName);
                }

                propertySchemaKvp.Value.SetName(transformedPropertyName, propertyName);
            }
        }
    }
}
