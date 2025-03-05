using System.Diagnostics.CodeAnalysis;

using Jeevan.ServiceCraftify.Transformers;
using Jeevan.ServiceCraftify.TypeProcessing;

using Microsoft.OpenApi.Models;

namespace Jeevan.ServiceCraftify;

public abstract class Generator<TSettings>
    where TSettings : GeneratorSettings
{
    [NotNull]
    public OpenApiDocument? OpenApiDoc { get; init; }

    [NotNull]
    public TSettings? Settings { get; init; }

    public IEnumerable<GeneratedCode> Generate()
    {
        RunDocumentProcessors();
        return GenerateCode();
    }

    protected abstract IEnumerable<GeneratedCode> GenerateCode();

    public void PrintDiagnosticInfo()
    {
        RunDocumentProcessors();

        foreach ((string route, OpenApiPathItem pathItem) in OpenApiDoc.Paths)
        {
            foreach ((OperationType method, OpenApiOperation operation) in pathItem.Operations)
            {
                Console.WriteLine($"{method.ToString().ToUpperInvariant()} {route}");
                Console.WriteLine($"    {operation.GetName()}.{operation.GetName(operation.OperationId)} <== {operation.OperationId}");
                foreach (OpenApiParameter parameter in operation.Parameters)
                    Console.WriteLine($"        [{parameter.In}] {parameter.GetName()} <== {parameter.Name}");
            }
        }

        foreach ((string modelName, OpenApiSchema modelSchema) in OpenApiDoc.Components.Schemas)
        {
            string typeName = modelSchema.GetSchemaTypeName();
            switch (typeName)
            {
                case ObjectSchemaType.Name:
                    Console.WriteLine($"object {modelSchema.GetName()} <== {modelName}");
                    foreach ((string propertyName, OpenApiSchema propertySchema) in modelSchema.Properties)
                        Console.WriteLine($"    {propertySchema.GetName(propertyName)} <== {propertyName}");
                    break;
                case EnumSchemaType.Name:
                    Console.WriteLine($"enum {modelSchema.GetName()} <== {modelName}");
                    EnumSchemaType enumSchemaType = (EnumSchemaType)modelSchema.GetSchemaType();
                    Console.WriteLine($"    Underlying type: {enumSchemaType.Details.UnderlyingType}");
                    foreach (EnumMember enumMember in enumSchemaType.Details.Members)
                        Console.WriteLine($"    {enumMember.Name} = {enumMember.Value}");
                    break;
                default:
                    Console.WriteLine($"err: Unrecognized schema type name {typeName}");
                    break;
            }
        }
    }

    protected virtual DocumentProcessor[] GetDocumentProcessors() => [];

    private void RunDocumentProcessors()
    {
        DocumentProcessor[] processors = GetDocumentProcessors();
        EnsureDocumentProcessorDependencies(processors);

        foreach (DocumentProcessor processor in processors)
            processor.Process();
    }

    private void EnsureDocumentProcessorDependencies(DocumentProcessor[] processors)
    {
        //TODO:
    }
}
