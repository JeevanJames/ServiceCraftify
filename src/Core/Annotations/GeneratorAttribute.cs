namespace Jeevan.ServiceCraftify.Annotations;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class GeneratorAttribute : Attribute
{
    public GeneratorAttribute(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public string Name { get; }

    public string? Language { get; set; }
}
