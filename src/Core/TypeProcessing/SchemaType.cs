namespace Jeevan.ServiceCraftify.TypeProcessing;

public abstract class SchemaType
{
    internal virtual bool Initialized { get; private set; }

    public void Decode(string encodedValue)
    {
        DecodeType(encodedValue);
        Initialized = true;
    }

    protected abstract void DecodeType(string encodedValue);

    public string Encode() => EncodeType();

    protected abstract string EncodeType();

    protected abstract string GetTypeName();

    public static SchemaType CreateSchemaTypeInstance(string schemaTypeName, string encodedType)
    {
        SchemaType schemaType = schemaTypeName switch
        {
            ObjectSchemaType.Name => new ObjectSchemaType(),
            EnumSchemaType.Name => new EnumSchemaType(),
            _ => throw new NotSupportedException($"Unrecognized schema type {schemaTypeName}"),
        };
        schemaType.Decode(encodedType);
        return schemaType;
    }
}

public class ObjectSchemaType : SchemaType
{
    public const string Name = "object";

    protected override void DecodeType(string encodedValue)
    {
    }

    protected override string EncodeType()
    {
        return string.Empty;

    }

    protected override string GetTypeName()
    {
        return Name;
    }
}
