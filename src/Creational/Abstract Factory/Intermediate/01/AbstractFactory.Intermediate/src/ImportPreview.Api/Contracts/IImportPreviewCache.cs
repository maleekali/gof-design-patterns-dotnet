namespace ImportPreview.Api.Contracts;

public interface IImportPreviewCache
{
    Task<ImportPreviewResponse?> GetAsync(string key, CancellationToken cancellationToken);
    Task SetAsync(string key, ImportPreviewResponse value, CancellationToken cancellationToken);
}
