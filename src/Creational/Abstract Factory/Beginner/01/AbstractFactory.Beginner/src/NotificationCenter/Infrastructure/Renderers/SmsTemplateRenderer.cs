using NotificationCenter.Abstractions;
using NotificationCenter.Infrastructure.Templates;

namespace NotificationCenter.Infrastructure.Renderers;

public sealed class SmsTemplateRenderer(FileTemplateStore templateStore) : ITemplateRenderer
{
    public async Task<string> RenderAsync(
        string templateName,
        IReadOnlyDictionary<string, string> values,
        CancellationToken cancellationToken = default)
    {
        TemplateNameValidator.Validate(templateName);
        var template = await templateStore.ReadAsync(
            Path.Combine("Sms", $"{templateName}.txt"),
            cancellationToken);

        return TemplateTokenReplacer.Replace(template, values, value => value);
    }
}
