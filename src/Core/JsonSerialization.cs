using System.Text.Json;

namespace Jeevan.ServiceCraftify;

internal static class JsonSerialization
{
    internal static readonly JsonSerializerOptions DefaultOptions = new()
    {
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never,
        WriteIndented = false,
    };
}
