namespace Jeevan.ServiceCraftify;

public sealed class GeneratedCode(string fileName, string code)
{
    public string Code { get; } = code ?? throw new ArgumentNullException(nameof(code));

    public string FileName { get; } = fileName ?? throw new ArgumentNullException(nameof(fileName));
}
