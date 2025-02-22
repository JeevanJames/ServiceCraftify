using System.Text.RegularExpressions;

using Humanizer;

using Jeevan.ServiceCraftify.Transformers;

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
        NameTransforms.RegexReplace(pattern, replacement)(name);

    public static ChildNameTransformer RegexReplace(string pattern, MatchEvaluator evaluator) => (name, _, _) =>
        NameTransforms.RegexReplace(pattern, evaluator)(name);

    public static ChildNameTransformer RegexReplace(Regex pattern, string replacement) => (name, _, _) =>
        NameTransforms.RegexReplace(pattern, replacement)(name);

    public static ChildNameTransformer RegexReplace(Regex pattern, MatchEvaluator evaluator) => (name, _, _) =>
        NameTransforms.RegexReplace(pattern, evaluator)(name);

    public static ChildNameTransformer Replace(string oldValue, string newValue) => (name, _, _) =>
        NameTransforms.Replace(oldValue, newValue)(name);

    public static ChildNameTransformer Strip(params string[] strs) => (name, _, _) =>
        NameTransforms.Strip(strs)(name);

    public static ChildNameTransformer StripPrefix(params string[] prefixes) => (name, _, _) =>
        NameTransforms.StripPrefix(prefixes)(name);

    public static ChildNameTransformer StripSuffix(params string[] suffixes) => (name, _, _) =>
        NameTransforms.StripSuffix(suffixes)(name);

    public static ChildNameTransformer Prefix(string prefix, params string[] patterns) => (name, _, _) =>
        NameTransforms.Prefix(prefix, patterns)(name);

    public static ChildNameTransformer Suffix(string suffix, params string[] patterns) => (name, _, _) =>
        NameTransforms.Suffix(suffix, patterns)(name);
}
