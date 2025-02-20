namespace Jeevan.ServiceCraftify;

public class GeneratorSettings
{
    private TransformerSettings? _transformers;

    /// <summary>
    ///     Provides a default name for services, if the name is not specified in the Open API document.<br/>
    ///     By default, the first tag name of the operation is considered to be the service name. If
    ///     the operation has no tags, then this value is used instead.
    /// </summary>
    public string DefaultServiceName { get; set; } = "Default";

    public TransformerSettings Transformers => _transformers ??= new TransformerSettings();
}

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

public delegate string NameTransformer(string name);
public delegate string ChildNameTransformer(string name, string transformedParentName, string originalParentName);
public delegate string ParameterNameTransformer(string name, string operationName, string serviceName);
