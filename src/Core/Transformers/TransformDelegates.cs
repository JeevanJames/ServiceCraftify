namespace Jeevan.ServiceCraftify.Transformers;

/// <summary>
///     Delegate to transform the names of top level items like models (Open API schemas) and services.
/// </summary>
public delegate string NameTransformer(string name);

/// <summary>
///     Delegate to transform the names of child level items like model properties (Open API schemas)
///     and service operations (Open API operations).
/// </summary>
public delegate string ChildNameTransformer(string name, string transformedParentName, string originalParentName);

public delegate string ParameterNameTransformer(string name, string operationName, string serviceName);
