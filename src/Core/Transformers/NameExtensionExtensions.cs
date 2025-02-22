using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;

namespace Jeevan.ServiceCraftify.Transformers;
#pragma warning disable S4136 // Method overloads should be grouped together
public static class NameExtensionExtensions
{
    private const string NameKey = "@@Name";

    public static string GetName(this IOpenApiExtensible extensible, string? childName = null)
    {
        string extensionKey = childName is null ? NameKey : $"{NameKey}_[{childName}]";
        if (!extensible.Extensions.TryGetValue(extensionKey, out IOpenApiExtension? entry))
            throw new CraftifyException($"No entry for {extensionKey}.");
        if (entry is not OpenApiString name)
            throw new CraftifyException($"Entry {extensionKey} with value {entry} is not a string.");
        return name.Value;
    }

    internal static void SetName(this IOpenApiExtensible extensible, string value, string? childName = null)
    {
        string extensionKey = childName is null ? NameKey : $"{NameKey}_[{childName}]";
        extensible.Extensions[extensionKey] = new OpenApiString(value);
    }
}
