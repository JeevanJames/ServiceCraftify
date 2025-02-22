using Microsoft.OpenApi.Models;

namespace Jeevan.ServiceCraftify.Transformers;

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
