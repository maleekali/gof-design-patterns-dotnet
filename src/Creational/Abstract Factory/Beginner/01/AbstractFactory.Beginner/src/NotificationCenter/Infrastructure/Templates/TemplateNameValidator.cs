namespace NotificationCenter.Infrastructure.Templates;

public static class TemplateNameValidator
{
    public static void Validate(string templateName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(templateName);

        if (templateName.Any(character =>
                !char.IsAsciiLetterOrDigit(character) && character is not '-' and not '_'))
        {
            throw new ArgumentException(
                "Template names may contain only ASCII letters, digits, hyphens, and underscores.",
                nameof(templateName));
        }
    }
}
