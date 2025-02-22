namespace Jeevan.ServiceCraftify.TypeProcessing;

public sealed class EnumSchemaType : SchemaType<EnumSchemaJsonModel>
{
    public const string Name = "enum";

    protected override string GetTypeName() => Name;
}

public sealed class EnumSchemaJsonModel
{
    public EnumUnderlyingType UnderlyingType { get; set; }

    public List<EnumMember> Members { get; set; } = new();
}

public sealed record EnumMember(string Name, int Value)
{
    public string? Description { get; set; }
}

public enum EnumUnderlyingType { String, Integer, Number };
