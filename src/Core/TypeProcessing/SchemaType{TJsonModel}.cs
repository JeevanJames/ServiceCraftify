using System.Text.Json;

namespace Jeevan.ServiceCraftify.TypeProcessing;

public abstract class SchemaType<TJsonModel> : SchemaType
    where TJsonModel : class, new()
{
    public TJsonModel Details { get; private set; } = new TJsonModel();

    protected override sealed void DecodeType(string encodedValue)
    {
        Details = JsonSerializer.Deserialize<TJsonModel>(encodedValue) ??
            throw new CraftifyException("Could not decode type data.");
    }

    protected override sealed string EncodeType() =>
        JsonSerializer.Serialize(Details, SerializerOptions);

#pragma warning disable S2743 // Static fields should not be used in generic types
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never,
        WriteIndented = false,
    };
#pragma warning restore S2743 // Static fields should not be used in generic types
}
