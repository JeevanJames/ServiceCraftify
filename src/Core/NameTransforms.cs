using System.Text.RegularExpressions;

using Humanizer;

namespace Jeevan.ServiceCraftify;

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

    public static NameTransformer RegexReplace(string pattern, string replacement) => name =>
        Regex.Replace(name, pattern, replacement);

    public static NameTransformer RegexReplace(string pattern, MatchEvaluator evaluator) => name =>
        Regex.Replace(name, pattern, evaluator);

    public static NameTransformer RegexReplace(Regex pattern, string replacement) => name =>
        pattern.Replace(name, replacement);

    public static NameTransformer RegexReplace(Regex pattern, MatchEvaluator evaluator) => name =>
        pattern.Replace(name, evaluator);

    public static NameTransformer Replace(string oldValue, string newValue) => name =>
        name.Replace(oldValue, newValue);

    public static NameTransformer Strip(params string[] strs) => name =>
    {
        string str = name;
        for (int i = 0; i < strs.Length; i++)
            str = str.Replace(strs[i], string.Empty);
        return str;
    };

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

#pragma warning disable S3358 // Ternary operators should not be nested
    public static NameTransformer Prefix(string prefix, params string[] patterns) => name =>
        patterns is null or { Length: 0 }
            ? prefix + name
            : Array.Exists(patterns, pattern => Regex.IsMatch(name, pattern)) ? prefix + name : name;

    public static NameTransformer Suffix(string suffix, params string[] patterns) => name =>
        patterns is null or { Length: 0 }
            ? name + suffix
            : Array.Exists(patterns, pattern => Regex.IsMatch(name, pattern)) ? name + suffix : name;
}
