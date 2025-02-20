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
        foreach (DocumentProcessor processor in GetDocumentProcessors())
            processor.Process();
        return GenerateCode();
    }

    protected abstract IEnumerable<GeneratedCode> GenerateCode();

    protected virtual IEnumerable<DocumentProcessor> GetDocumentProcessors()
    {
        yield break;
    }
}
