namespace NotificationCenter.Abstractions;

public interface ITemplateRenderer
{
    Task<string> RenderAsync(
        string templateName,
        IReadOnlyDictionary<string, string> values,
        CancellationToken cancellationToken = default);
}
