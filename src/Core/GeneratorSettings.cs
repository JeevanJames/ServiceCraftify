using System.Text.RegularExpressions;

using Humanizer;

namespace Jeevan.ServiceCraftify;

public class GeneratorSettings
{
    private TransformerSettings? _transformers;

    public TransformerSettings Transformers => _transformers ??= new TransformerSettings();

    public sealed class TransformerSettings
    {
        private List<NameTransformer>? _modelNames;
        private List<NameTransformer>? _propertyNames;

        private List<NameTransformer>? _serviceName;
        private List<NameTransformer>? _operationNames;
        private List<NameTransformer>? _parameterNames;

        public IList<NameTransformer> ModelNames => _modelNames ??= [];

        public IList<NameTransformer> PropertyNames => _propertyNames ??= [];

        public IList<NameTransformer> ServiceNames => _serviceName ??= [];

        public IList<NameTransformer> OperationNames => _operationNames ??= [];

        public IList<NameTransformer> ParameterNames => _parameterNames ??= [];
    }
}

public delegate string NameTransformer(string name);

public static class NameTransforms
{
    public static readonly NameTransformer PascalCase = name => name.Pascalize();
    public static readonly NameTransformer CamelCase = name => name.Camelize();
    public static readonly NameTransformer KebabCase = name => name.Kebaberize();
    public static readonly NameTransformer SnakeCase = name => name.Underscore();
    public static readonly NameTransformer UpperCase = name => name.ApplyCase(LetterCasing.AllCaps);
    public static readonly NameTransformer LowerCase = name => name.ApplyCase(LetterCasing.LowerCase);
    public static readonly NameTransformer SentenceCase = name => name.ApplyCase(LetterCasing.Sentence);
    public static readonly NameTransformer TitleCase = name => name.ApplyCase(LetterCasing.Title);

    public static NameTransformer RegexReplacement(string pattern, string replacement) => name =>
        Regex.Replace(name, pattern, replacement);

    public static NameTransformer RegexReplacement(string pattern, MatchEvaluator evaluator) => name =>
        Regex.Replace(name, pattern, evaluator);

    public static NameTransformer RegexReplacement(Regex pattern, string replacement) => name =>
        pattern.Replace(name, replacement);

    public static NameTransformer RegexReplacement(Regex pattern, MatchEvaluator evaluator) => name =>
        pattern.Replace(name, evaluator);

    public static NameTransformer StripPrefix(params string[] prefixes) => name =>
    {
        string? matchingPrefix = Array.Find(prefixes,
            prefix => name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
        return matchingPrefix is null ? name : name[matchingPrefix.Length..];
    };

    public static NameTransformer StripSuffix(params string[] suffixes) => name =>
    {
        string? matchingSuffix = Array.Find(suffixes,
            suffix => name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase));
        return matchingSuffix is null ? name : name[..^matchingSuffix.Length];
    };

    public static NameTransformer Prefix(string prefix, params string[] patterns) => name =>
        Array.Exists(patterns, pattern => Regex.IsMatch(name, pattern)) ? prefix + name : name;

    public static NameTransformer Suffix(string suffix, params string[] patterns) => name =>
        Array.Exists(patterns, pattern => Regex.IsMatch(name, pattern)) ? name + suffix : name;
}
