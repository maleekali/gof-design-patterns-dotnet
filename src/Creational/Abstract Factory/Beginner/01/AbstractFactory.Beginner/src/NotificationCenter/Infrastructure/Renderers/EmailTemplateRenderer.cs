using System.Net;
using NotificationCenter.Abstractions;
using NotificationCenter.Infrastructure.Templates;

namespace NotificationCenter.Infrastructure.Renderers;

public sealed class EmailTemplateRenderer(FileTemplateStore templateStore) : ITemplateRenderer
{
    public async Task<string> RenderAsync(
        string templateName,
        IReadOnlyDictionary<string, string> values,
        CancellationToken cancellationToken = default)
    {
        TemplateNameValidator.Validate(templateName);
        var template = await templateStore.ReadAsync(
            Path.Combine("Email", $"{templateName}.html"),
            cancellationToken);

        return TemplateTokenReplacer.Replace(template, values, WebUtility.HtmlEncode);
    }
}
