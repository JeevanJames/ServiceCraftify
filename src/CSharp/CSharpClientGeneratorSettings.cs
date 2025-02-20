
namespace Jeevan.ServiceCraftify.CSharp;

public sealed class CSharpClientGeneratorSettings : GeneratorSettings
{
    public required string ClientNamespace { get; init; }

    public string? ModelNamespace { get; set; }

    public ConsolidatedClientGeneration ConsolidatedClientGeneration { get; set; }

    public string? ConsolidatedClientName { get; set; }

    public bool AddExtensionDataForModels { get; set; }
}

public enum ConsolidatedClientGeneration
{
    IfMultipleClients,
    Never,
    Always,
}
