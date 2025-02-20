using System.Text.RegularExpressions;

using Humanizer;

namespace Jeevan.ServiceCraftify;

public static class ChildNameTransforms
{
    public static readonly ChildNameTransformer PascalCase = (name, _, _) => name.Pascalize();
    public static readonly ChildNameTransformer CamelCase = (name, _, _) => name.Camelize();
    public static readonly ChildNameTransformer KebabCase = (name, _, _) => name.Kebaberize();
    public static readonly ChildNameTransformer SnakeCase = (name, _, _) => name.Underscore();
    public static readonly ChildNameTransformer UpperCase = (name, _, _) => name.ApplyCase(LetterCasing.AllCaps);
    public static readonly ChildNameTransformer LowerCase = (name, _, _) => name.ApplyCase(LetterCasing.LowerCase);
    public static readonly ChildNameTransformer SentenceCase = (name, _, _) => name.ApplyCase(LetterCasing.Sentence);
    public static readonly ChildNameTransformer TitleCase = (name, _, _) => name.ApplyCase(LetterCasing.Title);

    public static ChildNameTransformer RegexReplace(string pattern, string replacement) => (name, _, _) =>
        Regex.Replace(name, pattern, replacement);

    public static ChildNameTransformer RegexReplace(string pattern, MatchEvaluator evaluator) => (name, _, _) =>
        Regex.Replace(name, pattern, evaluator);

    public static ChildNameTransformer RegexReplace(Regex pattern, string replacement) => (name, _, _) =>
        pattern.Replace(name, replacement);

    public static ChildNameTransformer RegexReplace(Regex pattern, MatchEvaluator evaluator) => (name, _, _) =>
        pattern.Replace(name, evaluator);

    public static ChildNameTransformer Replace(string oldValue, string newValue) => (name, _, _) =>
        name.Replace(oldValue, newValue);

    public static ChildNameTransformer Strip(params string[] strs) => (name, _, _) =>
    {
        string str = name;
        for (int i = 0; i < strs.Length; i++)
            str = str.Replace(strs[i], string.Empty);
        return str;
    };

    public static ChildNameTransformer StripPrefix(params string[] prefixes) => (name, _, _) =>
    {
        string? matchingPrefix = Array.Find(prefixes,
            prefix => name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
        return matchingPrefix is null ? name : name[matchingPrefix.Length..];
    };

    public static ChildNameTransformer StripSuffix(params string[] suffixes) => (name, _, _) =>
    {
        string? matchingSuffix = Array.Find(suffixes,
            suffix => name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase));
        return matchingSuffix is null ? name : name[..^matchingSuffix.Length];
    };

#pragma warning disable S3358 // Ternary operators should not be nested
    public static ChildNameTransformer Prefix(string prefix, params string[] patterns) => (name, _, _) =>
        patterns is null or { Length: 0 }
            ? prefix + name
            : Array.Exists(patterns, pattern => Regex.IsMatch(name, pattern)) ? prefix + name : name;

    public static ChildNameTransformer Suffix(string suffix, params string[] patterns) => (name, _, _) =>
        patterns is null or { Length: 0 }
            ? name + suffix
            : Array.Exists(patterns, pattern => Regex.IsMatch(name, pattern)) ? name + suffix : name;
}
