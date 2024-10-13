namespace Jeevan.ServiceCraftify;

public abstract class Generator<TBindingModel, TSettings>
    where TBindingModel : BindingModel
    where TSettings : GeneratorSettings
{
    protected Generator(TSettings settings)
    {
        Settings = settings;
    }

    public TSettings Settings { get; }

    public abstract IEnumerable<GeneratedCode> Generate(TBindingModel bindingModel);
}

public sealed class GeneratedCode
{
    public GeneratedCode(string fileName, string code)
    {
        Code = code ?? throw new ArgumentNullException(nameof(code));
        FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
    }

    public string Code { get; }

    public string FileName { get; }
}
