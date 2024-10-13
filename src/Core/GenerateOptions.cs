namespace Jeevan.ServiceCraftify;

public abstract class GenerateOptions(string generatorPackage)
{
    public string GeneratorPackage { get; } = generatorPackage;

    public string? PackageSource { get; set; }

    /// <summary>
    ///     The name of the generator in the generator package. This property is not needed if:<br/>
    ///     o The package contains only one generator.<br/>
    ///     o The package contains multiple generators, but a default generator is defined and you want
    ///       to use that.
    /// </summary>
    /// <value></value>
    public string? GeneratorName { get; set; }

    public static FileGenerateOptions FromFile(FileInfo openApiFile, string generatorPackage,
        Action<FileGenerateOptions> optionsBuilder)
    {
        FileGenerateOptions options = new(openApiFile, generatorPackage);
        optionsBuilder(options);
        return options;
    }

    public static UriGenerateOptions FromUri(Uri openApiUri, string generatorPackage,
        Action<UriGenerateOptions> optionsBuilder)
    {
        UriGenerateOptions options = new(openApiUri, generatorPackage);
        optionsBuilder(options);
        return options;
    }
}

public sealed class FileGenerateOptions : GenerateOptions
{
    public FileGenerateOptions(FileInfo openApiFile, string generatorPackage)
        : base(generatorPackage)
    {
        ArgumentNullException.ThrowIfNull(openApiFile);
        if (!File.Exists(openApiFile.FullName))
            throw new FileNotFoundException($"Specified OpenAPI file '{openApiFile.FullName}' could not be found.", openApiFile.FullName);
        OpenApiFile = openApiFile;
    }

    public FileInfo OpenApiFile { get; }
}

public sealed class UriGenerateOptions : GenerateOptions
{
    public UriGenerateOptions(Uri openApiUri, string generatorPackage)
        : base(generatorPackage)
    {
        ArgumentNullException.ThrowIfNull(openApiUri);
        if (!openApiUri.IsAbsoluteUri)
            throw new ArgumentException($"Specified OpenAPI URI '{openApiUri}' is not a absolute URI.", nameof(openApiUri));
        OpenApiUri = openApiUri;
    }

    public Uri OpenApiUri { get; }
}
