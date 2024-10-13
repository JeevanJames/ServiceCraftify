using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;

namespace Jeevan.ServiceCraftify.Implementations;

internal sealed class GenerateImplementation
{
#pragma warning disable S4487 // Unread "private" fields should be removed
    private readonly OpenApiDocument _openApiDoc;
    private readonly GenerateOptions _options;
#pragma warning restore S4487 // Unread "private" fields should be removed

    private GenerateImplementation(OpenApiDocument openApiDoc, GenerateOptions options)
    {
        _openApiDoc = openApiDoc;
        _options = options;
    }

    internal static async Task<GenerateImplementation> Create(GenerateOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        Stream openApiStream;
        switch (options)
        {
            case FileGenerateOptions fgo:
                openApiStream = new FileStream(fgo.OpenApiFile.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
                break;
            case UriGenerateOptions ugo:
                HttpClient client = new();
                openApiStream = await client.GetStreamAsync(ugo.OpenApiUri);
                break;
            default:
                throw new NotSupportedException($"Unsupported {nameof(GenerateOptions)} type {options.GetType()}.");
        }

        OpenApiDocument openApiDoc = new OpenApiStreamReader().Read(openApiStream, out OpenApiDiagnostic diagnostic);
        return new GenerateImplementation(openApiDoc, options);
    }

#pragma warning disable S2325 // Methods and properties that don't access instance data should be static
    internal Task<string> GenerateAsync()
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static
    {
        /*
          1. Get generator package
             - Check latest version of package
             - Get from cache if available
             - Otherwise get from source
             - Extract generator assembly from the package and copy to well-known sub-directory
          2.
        */

        throw new NotImplementedException();
    }
}
