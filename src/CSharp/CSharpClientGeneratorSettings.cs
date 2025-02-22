using Jeevan.ServiceCraftify.Transformers;

namespace Jeevan.ServiceCraftify.CSharp;

public sealed class CSharpClientGeneratorSettings : GeneratorSettings
{
    private ConsolidatedClientSettings? _consolidatedClient;

    public required string Namespace { get; init; }

    public ConsolidatedClientSettings ConsolidatedClient
    {
        get => _consolidatedClient ??= new ConsolidatedClientSettings();
        set => _consolidatedClient = value ?? throw new ArgumentNullException(nameof(value));
    }

    public bool AddExtensionDataForModels { get; set; }
}

public sealed class ConsolidatedClientSettings
{
    public ConsolidatedClientGeneration Generation { get; set; }

    public string Name { get; set; } = "Consolidated";

    public NameTransformer[]? PropertyNameTransforms { get; set; }
}

public enum ConsolidatedClientGeneration
{
    IfMultipleClients,
    Never,
    Always,
}
