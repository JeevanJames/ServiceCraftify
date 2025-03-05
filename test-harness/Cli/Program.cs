using Humanizer;

using Jeevan.ServiceCraftify;
using Jeevan.ServiceCraftify.CSharp;

using NuGet.Packaging;

#pragma warning disable CS8321 // Local function is declared but never used
#pragma warning disable S1075 // URIs should not be hardcoded

//Craftify craftify = await Craftify.FromUri(new Uri("https://petstore.swagger.io/v2/swagger.json", UriKind.Absolute));
Craftify craftify = await Craftify.FromFile(@"D:\Temp\OpenApi\Sample01.yml");
craftify.EnsureNoErrors(failOnWarnings: true);

GeneratedCode code = RunCSharpGenerator(craftify);
//GeneratedCode code = RunPseudoGenerator(craftify);

Console.WriteLine(code.FileName);
Console.WriteLine(code.Code);

static GeneratedCode RunCSharpGenerator(Craftify craftify)
{
    CSharpClientGeneratorSettings settings = new()
    {
        Namespace = "MyNamespace",
        ConsolidatedClient =
        {
            Name = "ConsolidatedClient",
            PropertyNameTransforms = [NameTransforms.StripSuffix(["Client"])]
        }
    };
    settings.Transformers.ServiceNames.AddRange(
    [
        NameTransforms.PascalCase(),
        NameTransforms.Suffix("Client"),
    ]);
    settings.Transformers.OperationNames.Add(ChildNameTransforms.PascalCase);
    settings.Transformers.ParameterNames.Add((n, _, _) => n.Underscore());
    settings.Transformers.ModelNames.AddRange(
    [
        NameTransforms.Strip(["."]),
        NameTransforms.StripPrefix(["Get"]),
    ]);
    settings.Transformers.PropertyNames.AddRange(
    [
        ChildNameTransforms.PascalCase,
        ChildNameTransforms.RegexReplace(@"^URL(\w+)$", "Url$1"),
    ]);

    craftify.PrintDiagnosticInfo<CSharpClientGenerator, CSharpClientGeneratorSettings>(settings);
    IEnumerable<GeneratedCode> code = craftify.Generate<CSharpClientGenerator, CSharpClientGeneratorSettings>(settings);
    return code.First();
}

static GeneratedCode RunPseudoGenerator(Craftify craftify)
{
    GeneratorSettings settings = new() { DefaultServiceName = "TheDefault" };
    settings.Transformers.ServiceNames.AddRange(
        [
            NameTransforms.PascalCase(),
            NameTransforms.Suffix("Client"),
        ]);
    settings.Transformers.OperationNames.AddRange(
        [
            ChildNameTransforms.PascalCase,
        ]);
    IEnumerable<GeneratedCode> code = craftify.Generate<PseudoCodeGenerator, GeneratorSettings>(settings);
    return code.First();
}
