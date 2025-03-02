using System.Text.RegularExpressions;

using Humanizer;

using Jeevan.ServiceCraftify.Transformers;

namespace Jeevan.ServiceCraftify;

public static class NameTransforms
{
    public static NameTransformer PascalCase(params string[] patterns) => name =>
        TransformForPatterns(patterns, name, n => n.Pascalize());

    public static NameTransformer CamelCase(params string[] patterns) => name =>
        TransformForPatterns(patterns, name, n => n.Camelize());

    public static NameTransformer KebabCase(params string[] patterns) => name =>
        TransformForPatterns(patterns, name, n => n.Kebaberize());

    public static NameTransformer SnakeCase(params string[] patterns) => name =>
        TransformForPatterns(patterns, name, n => n.Underscore());

    public static NameTransformer UpperCase(params string[] patterns) => name =>
        TransformForPatterns(patterns, name, n => n.ApplyCase(LetterCasing.AllCaps));

    public static NameTransformer LowerCase(params string[] patterns) => name =>
        TransformForPatterns(patterns, name, n => n.ApplyCase(LetterCasing.LowerCase));

    public static NameTransformer SentenceCase(params string[] patterns) => name =>
        TransformForPatterns(patterns, name, n => n.ApplyCase(LetterCasing.Sentence));

    public static NameTransformer TitleCase(params string[] patterns) => name =>
        TransformForPatterns(patterns, name, n => n.ApplyCase(LetterCasing.Title));

    public static NameTransformer RegexReplace(string pattern, string replacement) => name =>
        Regex.Replace(name, pattern, replacement);

    public static NameTransformer RegexReplace(string pattern, MatchEvaluator evaluator) => name =>
        Regex.Replace(name, pattern, evaluator);

    public static NameTransformer RegexReplace(Regex  pattern, string replacement) => name =>
        pattern.Replace(name, replacement);

    public static NameTransformer RegexReplace(Regex pattern, MatchEvaluator evaluator) => name =>
        pattern.Replace(name, evaluator);

    public static NameTransformer Replace(string oldValue, string newValue, params string[] patterns) => name =>
        TransformForPatterns(patterns, name, (oldValue, newValue), static (n, args) =>
            n.Replace(args.oldValue, args.newValue));

    public static NameTransformer Strip(string[] strs, params string[] patterns) => name =>
        TransformForPatterns(patterns, name, strs, static (n, arg) =>
        {
            string str = n;
            for (int i = 0; i < arg.Length; i++)
                str = str.Replace(arg[i], string.Empty);
            return str;
        });

    public static NameTransformer StripPrefix(string[] prefixes, params string[] patterns) => name =>
        TransformForPatterns(patterns, name, prefixes, static (n, arg) =>
        {
            string? matchingPrefix = Array.Find(arg,
                prefix => n.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
            return matchingPrefix is null ? n : n[matchingPrefix.Length..];
        });

    public static NameTransformer StripSuffix(string[] suffixes, params string[] patterns) => name =>
        TransformForPatterns(patterns, name, suffixes, static (n, arg) =>
        {
            string? matchingSuffix = Array.Find(arg,
                suffix => n.EndsWith(suffix, StringComparison.OrdinalIgnoreCase));
            return matchingSuffix is null ? n : n[..^matchingSuffix.Length];
        });

    public static NameTransformer Prefix(string prefix, params string[] patterns) => name =>
        TransformForPatterns(patterns, name, prefix, static (n, arg) => arg + n);

    public static NameTransformer Suffix(string suffix, params string[] patterns) => name =>
        TransformForPatterns(patterns, name, suffix, static (n, arg) => n + arg);

    private static string TransformForPatterns(string[]? patterns, string name, Func<string, string> function)
    {
        if (patterns is null or { Length: 0 })
            return function(name);
        return Array.Exists(patterns, pattern => Regex.IsMatch(name, pattern)) ? function(name) : name;
    }

    private static string TransformForPatterns<TArg>(string[]? patterns, string name, TArg arg,
        Func<string, TArg, string> function)
    {
        if (patterns is null or { Length: 0 })
            return function(name, arg);
        return Array.Exists(patterns, pattern => Regex.IsMatch(name, pattern)) ? function(name, arg) : name;
    }
}
