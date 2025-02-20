using Jeevan.ServiceCraftify;
using Jeevan.ServiceCraftify.CSharp;

using NuGet.Packaging;

#pragma warning disable CS8321 // Local function is declared but never used
#pragma warning disable S1075 // URIs should not be hardcoded

Craftify craftify = await Craftify.FromUri(new Uri("https://petstore.swagger.io/v2/swagger.json", UriKind.Absolute));
//Craftify craftify = await Craftify.FromFile(@"D:\Temp\OpenApi\Egdmr_Ele_3.0.yml");
craftify.EnsureNoErrors(failOnWarnings: true);

GeneratedCode code = RunCSharpGenerator(craftify);
//GeneratedCode code = RunPseudoGenerator(craftify);

Console.WriteLine(code.FileName);
Console.WriteLine(code.Code);

static GeneratedCode RunCSharpGenerator(Craftify craftify)
{
    CSharpClientGeneratorSettings settings = new()
    {
        ClientNamespace = "MyNamespace",
        ModelNamespace = "MyNamespace",
        ConsolidatedClientName = "ConsolidatedClient",
    };
    settings.Transformers.ServiceNames.AddRange(
        [
            NameTransforms.PascalCase,
            NameTransforms.Suffix("Client"),
        ]);
    settings.Transformers.ModelNames.AddRange(
    [
        NameTransforms.Strip("."),
        NameTransforms.StripPrefix("Get"),
    ]);
    settings.Transformers.PropertyNames.Add(ChildNameTransforms.SnakeCase);

    //IEnumerable<GeneratedCode> code = craftify.Generate<PseudoCodeGenerator, GeneratorSettings>(settings);
    IEnumerable<GeneratedCode> code = craftify.Generate<CSharpClientGenerator, CSharpClientGeneratorSettings>(settings);
    return code.First();
}

static GeneratedCode RunPseudoGenerator(Craftify craftify)
{
    GeneratorSettings settings = new() { DefaultServiceName = "TheDefault" };
    settings.Transformers.ServiceNames.AddRange(
        [
            NameTransforms.PascalCase,
            NameTransforms.Suffix("Client"),
        ]);
    settings.Transformers.OperationNames.AddRange(
        [
            ChildNameTransforms.PascalCase,
        ]);
    IEnumerable<GeneratedCode> code = craftify.Generate<PseudoCodeGenerator, GeneratorSettings>(settings);
    return code.First();
}
