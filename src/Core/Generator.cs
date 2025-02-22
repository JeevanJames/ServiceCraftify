using System.Diagnostics.CodeAnalysis;

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
        DocumentProcessor[] processors = GetDocumentProcessors();
        EnsureDocumentProcessorDependencies(processors);

        foreach (DocumentProcessor processor in processors)
            processor.Process();

        return GenerateCode();
    }

    protected abstract IEnumerable<GeneratedCode> GenerateCode();

    protected virtual DocumentProcessor[] GetDocumentProcessors() => [];

    private void EnsureDocumentProcessorDependencies(DocumentProcessor[] processors)
    {
        //TODO:
    }
}
