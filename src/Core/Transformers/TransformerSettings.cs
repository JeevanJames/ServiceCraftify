namespace Jeevan.ServiceCraftify.Transformers;

public sealed class TransformerSettings
{
    private List<NameTransformer>? _modelNames;
    private List<ChildNameTransformer>? _propertyNames;

    private List<NameTransformer>? _serviceNames;
    private List<ChildNameTransformer>? _operationNames;
    private List<ParameterNameTransformer>? _parameterNames;

    public bool HasModelNameTransformers => _modelNames is { Count: > 0 };
    public IList<NameTransformer> ModelNames => _modelNames ??= [];

    public bool HasPropertyNameTransformers => _propertyNames is { Count: > 0 };
    public IList<ChildNameTransformer> PropertyNames => _propertyNames ??= [];

    public bool HasServiceNameTransformers => _serviceNames is { Count: > 0 };
    public IList<NameTransformer> ServiceNames => _serviceNames ??= [];

    public bool HasOperationNameTransformers => _operationNames is { Count: > 0 };
    public IList<ChildNameTransformer> OperationNames => _operationNames ??= [];

    public bool HasParameterNameTransformers => _parameterNames is { Count: > 0 };
    public IList<ParameterNameTransformer> ParameterNames => _parameterNames ??= [];
}
