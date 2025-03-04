using Microsoft.OpenApi.Interfaces;

namespace Jeevan.ServiceCraftify.Transformers;

public static class NameExtensionExtensions
{
    private const string NameKey = "@@Name";

    public static string GetName(this IOpenApiExtensible extensible, string? childName = null) =>
        extensible.GetExtensionValue(NameKey, childName);

    internal static void SetName(this IOpenApiExtensible extensible, string value, string? childName = null) =>
        extensible.SetExtensionValue(value, NameKey, childName);
}
