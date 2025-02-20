using System.Text;

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

        foreach (KeyValuePair<string, OpenApiSchema> schemaKvp in OpenApiDoc.Components.Schemas)
        {
            sb.AppendLine(schemaKvp.GetName());
            foreach (KeyValuePair<string, OpenApiSchema> propertySchemaKvp in schemaKvp.Value.Properties)
            {
                sb.AppendLine($"    {propertySchemaKvp.GetName()}: {propertySchemaKvp.Value.Type}");
            }
        }

        yield return new GeneratedCode("Client.txt", sb.ToString());
    }

    protected override IEnumerable<DocumentProcessor> GetDocumentProcessors()
    {
        yield return new TransformDocumentProcessor(OpenApiDoc, Settings);
    }
}
