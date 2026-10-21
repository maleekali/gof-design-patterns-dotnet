using System.Text.RegularExpressions;

namespace NotificationCenter.Infrastructure.Templates;

public static partial class TemplateTokenReplacer
{
    [GeneratedRegex(@"\{\{([A-Za-z][A-Za-z0-9_]*)\}\}", RegexOptions.CultureInvariant)]
    private static partial Regex TokenRegex();

    public static string Replace(
        string template,
        IReadOnlyDictionary<string, string> values,
        Func<string, string> transformValue)
    {
        ArgumentNullException.ThrowIfNull(template);
        ArgumentNullException.ThrowIfNull(values);
        ArgumentNullException.ThrowIfNull(transformValue);

        var result = TokenRegex().Replace(template, match =>
        {
            var key = match.Groups[1].Value;
            if (!values.TryGetValue(key, out var value))
            {
                throw new InvalidOperationException($"Template value '{key}' was not provided.");
            }

            return transformValue(value);
        });

        if (result.Contains("{{", StringComparison.Ordinal) || result.Contains("}}", StringComparison.Ordinal))
        {
            throw new InvalidOperationException("The template contains an invalid or unresolved placeholder.");
        }

        return result;
    }
}
