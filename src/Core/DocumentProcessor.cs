using Microsoft.OpenApi.Models;

namespace Jeevan.ServiceCraftify;

public abstract class DocumentProcessor(OpenApiDocument document)
{
    public OpenApiDocument Document { get; } = document ?? throw new ArgumentNullException(nameof(document));

    public abstract void Process();

    public virtual Type[] GetDependentProcessors() => [];
}

public abstract class DocumentProcessor<TSettings>(OpenApiDocument document, TSettings settings)
    : DocumentProcessor(document)
    where TSettings : GeneratorSettings
{
    public TSettings Settings { get; } = settings ?? throw new ArgumentNullException(nameof(settings));
}
