using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;

namespace Jeevan.ServiceCraftify;

public static class ExtensibleExtensionExtensions
{
    public static string GetExtensionValue<TOpenApiExtensible>(this TOpenApiExtensible extensible,
        string key, string? subkey = null)
        where TOpenApiExtensible : IOpenApiExtensible
    {
        ArgumentNullException.ThrowIfNull(extensible);

        string finalKey = string.IsNullOrWhiteSpace(subkey) ? key : $"{key}_{subkey}";
        if (!extensible.Extensions.TryGetValue(finalKey, out IOpenApiExtension? extension))
            throw new CraftifyException($"No key named {finalKey} was found for extensible type {typeof(TOpenApiExtensible).Name}.");
        if (extension is not OpenApiString stringExtension)
            throw new CraftifyException($"Extension with key {finalKey} on extensible type {typeof(TOpenApiExtensible).Name} is not a string.");
        return stringExtension.Value;
    }

    public static TResult GetExtensionValue<TOpenApiExtensible, TResult>(this TOpenApiExtensible extensible,
        string key, string? subkey = null, Func<string, TResult>? converter = null)
        where TOpenApiExtensible : IOpenApiExtensible
    {
        string value = extensible.GetExtensionValue(key, subkey);

        converter ??= DefaultConverter;
        return converter(value);

        static TResult DefaultConverter(string value) =>
            JsonSerializer.Deserialize<TResult>(value, _serializerOptions) ??
                throw new CraftifyException($"Could not deserialize JSON value from extension type {typeof(TOpenApiExtensible).Name}.");
    }

    public static void SetExtensionValue<TOpenApiExtensible>(this TOpenApiExtensible extensible, string value,
        string key, string? subkey = null)
        where TOpenApiExtensible : IOpenApiExtensible
    {
        ArgumentNullException.ThrowIfNull(extensible);

        string finalKey = string.IsNullOrWhiteSpace(subkey) ? key : $"{key}_{subkey}";
        extensible.Extensions[finalKey] = new OpenApiString(value);
    }

    public static void SetExtensionValue<TOpenApiExtensible, TValue>(this TOpenApiExtensible extensible, TValue value,
        string key, string? subkey = null, Func<TValue, string>? converter = null)
        where TOpenApiExtensible : IOpenApiExtensible
    {
        converter ??= DefaultConverter;
        string stringValue = converter(value);
        extensible.SetExtensionValue(stringValue, key, subkey);

        static string DefaultConverter(TValue value) => JsonSerializer.Serialize(value, _serializerOptions);
    }

    private static readonly JsonSerializerOptions _serializerOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        WriteIndented = false,
    };
}

