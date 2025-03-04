using System.Text.Json;

namespace Jeevan.ServiceCraftify.TypeProcessing;

public abstract class SchemaType<TJsonModel> : SchemaType
    where TJsonModel : class, new()
{
    public TJsonModel Details { get; private set; } = new TJsonModel();

    protected override sealed void DecodeType(string encodedValue)
    {
        Details = JsonSerializer.Deserialize<TJsonModel>(encodedValue, JsonSerialization.DefaultOptions) ??
            throw new CraftifyException("Could not decode type data.");
    }

    protected override sealed string EncodeType() =>
        JsonSerializer.Serialize(Details, JsonSerialization.DefaultOptions);
}
