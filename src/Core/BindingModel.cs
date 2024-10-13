using Microsoft.OpenApi.Models;

namespace Jeevan.ServiceCraftify;

public abstract class BindingModel
{
    protected BindingModel(OpenApiDocument openApiDoc)
    {
        OpenApiDoc = openApiDoc;
    }

    public OpenApiDocument OpenApiDoc { get; }
}

#pragma warning disable S1694 // An abstract class should have both abstract and concrete methods
public abstract class BindingModelFactory<TBindingModel>
#pragma warning restore S1694 // An abstract class should have both abstract and concrete methods
    where TBindingModel : BindingModel
{
    public abstract TBindingModel Create(OpenApiDocument openApiDoc);
}
