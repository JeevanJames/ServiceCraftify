using System.Text;

using Jeevan.ServiceCraftify.Transformers;
using Jeevan.ServiceCraftify.TypeProcessing;

using Microsoft.OpenApi.Models;

namespace Jeevan.ServiceCraftify;

public sealed class PseudoCodeGenerator : Generator<GeneratorSettings>
{
    protected override IEnumerable<GeneratedCode> GenerateCode()
    {
        StringBuilder sb = new();

        foreach ((string route, OpenApiPathItem pathItem) in OpenApiDoc.Paths)
        {
            foreach ((OperationType method, OpenApiOperation operation) in pathItem.Operations)
            {
                sb.AppendLine($"{method.ToString().ToUpperInvariant()} {route}");

                string serviceName = operation.GetName();
                string operationName = operation.GetName(operation.OperationId);

                sb.Append("    ").Append(serviceName).Append('.').Append(operationName);
                foreach (OpenApiParameter parameter in operation.Parameters)
                    sb.Append($" | [{parameter.In}]").Append(parameter.Name).Append(": ").Append(parameter.Schema.Type);
                sb.AppendLine();
            }
        }

        sb.AppendLine("======================");

        foreach (OpenApiSchema schema in OpenApiDoc.Components.Schemas.Values)
        {
            sb.AppendLine(schema.GetName());
            foreach (OpenApiSchema propertySchema in schema.Properties.Values)
            {
                sb.AppendLine($"    {propertySchema.GetName()}: {propertySchema.Type}");
            }
        }

        yield return new GeneratedCode("Client.txt", sb.ToString());
    }

    protected override DocumentProcessor[] GetDocumentProcessors() => [
        new TransformDocumentProcessor(OpenApiDoc, Settings),
        new TypeDocumentProcessor(OpenApiDoc),
    ];
}
