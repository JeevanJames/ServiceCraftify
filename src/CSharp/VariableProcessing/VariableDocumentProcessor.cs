using System.Diagnostics;

using Microsoft.OpenApi.Models;

namespace Jeevan.ServiceCraftify.CSharp.VariableProcessing;

public sealed class VariableDocumentProcessor(OpenApiDocument document) : DocumentProcessor(document)
{
    public override void Process()
    {
        ProcessParameters();
    }

    private void ProcessParameters()
    {
        foreach (OpenApiPathItem pathItem in Document.Paths.Values)
        {
            Debug.Assert(pathItem is not null);
        }
    }
}
