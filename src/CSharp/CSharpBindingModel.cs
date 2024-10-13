using Microsoft.OpenApi.Models;

namespace Jeevan.ServiceCraftify.CSharp;

public sealed class CSharpBindingModel : BindingModel
{
    public CSharpBindingModel(OpenApiDocument openApiDoc)
        : base(openApiDoc)
    {
    }
}
