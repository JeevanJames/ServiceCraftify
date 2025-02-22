using Jeevan.ServiceCraftify.Transformers;

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

    /// <summary>
    ///     Transformers for names of Open API artefacts such as services, operations, operation parameters,
    ///     models and model properties.
    /// </summary>
    public TransformerSettings Transformers => _transformers ??= new TransformerSettings();
}
