using System.Text;

using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;

namespace Jeevan.ServiceCraftify;

/// <summary>
///     Starting point to use the ServiceCraftify framework. Use this class to load an Open API or Swagger
///     document using one of either the constructors of the <c>FromXXXX</c> factory methods.
/// </summary>
public sealed class Craftify
{
    private readonly OpenApiDocument _openApiDoc;

    /// <summary>
    ///     Errors and warnings related to the loading of the Open API document.
    /// </summary>
    public OpenApiDiagnostic Diagnostic { get; }

    public Craftify(Stream openApiStream)
    {
        ArgumentNullException.ThrowIfNull(openApiStream);
        _openApiDoc = new OpenApiStreamReader().Read(openApiStream, out OpenApiDiagnostic diagnostic);
        Diagnostic = diagnostic;
    }

    public Craftify(string openApiString)
    {
        _openApiDoc = new OpenApiStringReader().Read(openApiString, out OpenApiDiagnostic diagnostic);
        Diagnostic = diagnostic;
    }

    public Craftify(TextReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);
        _openApiDoc = new OpenApiTextReaderReader().Read(reader, out OpenApiDiagnostic diagnostic);
        Diagnostic = diagnostic;
    }

    public static async Task<Craftify> FromUri(Uri openApiUri, HttpClient? httpClient = null)
    {
        ArgumentNullException.ThrowIfNull(openApiUri);

        httpClient ??= new HttpClient();
        await using Stream openApiStream = await httpClient.GetStreamAsync(openApiUri);
        return new Craftify(openApiStream);
    }

    public static async Task<Craftify> FromFile(string filePath)
    {
        ArgumentNullException.ThrowIfNull(filePath);
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"OpenAPI file '{filePath}' not found.", filePath);

        await using FileStream fs = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return new Craftify(fs);
    }

    public static async Task<Craftify> FromFile(FileInfo file)
    {
        ArgumentNullException.ThrowIfNull(file);
        if (!file.Exists)
            throw new FileNotFoundException($"OpenAPI file '{file.FullName}' not found.", file.FullName);

        await using FileStream fs = new(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
        return new Craftify(fs);
    }

    /// <summary>
    ///     Throws an exception if there were errors when loading the Open API document.
    /// </summary>
    /// <param name="failOnWarnings">
    ///     Throw an exception even if there are only warnings from loading the Open API document.
    /// </param>
    /// <exception cref="CraftifyException">
    ///     Thrown if there were errors when loading the Open API document.
    /// </exception>
    public void EnsureNoErrors(bool failOnWarnings = false)
    {
        StringBuilder? errorMessage = null;

        if (Diagnostic.Errors.Count > 0)
        {
            errorMessage = Diagnostic.Errors.Aggregate(new StringBuilder(),
                (sb, err) => sb.Append('[').Append(err.Pointer).Append("] ").AppendLine(err.Message));
        }

        if (failOnWarnings && Diagnostic.Warnings.Count > 0)
        {
            errorMessage = Diagnostic.Warnings.Aggregate(errorMessage ?? new StringBuilder(),
                (sb, err) => sb.Append('[').Append(err.Pointer).Append("] ").AppendLine(err.Message));
        }

        if (errorMessage is not null)
            throw new CraftifyException(errorMessage.ToString());
    }

    public IEnumerable<GeneratedCode> Generate<TGenerator, TSettings>(TSettings settings)
        where TGenerator : Generator<TSettings>, new()
        where TSettings : GeneratorSettings
    {
        TGenerator generator = new()
        {
            OpenApiDoc = _openApiDoc,
            Settings = settings,
        };
        return generator.Generate();
    }
}
