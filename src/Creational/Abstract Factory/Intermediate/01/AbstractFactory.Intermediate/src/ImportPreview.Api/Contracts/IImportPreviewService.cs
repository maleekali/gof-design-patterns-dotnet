namespace ImportPreview.Api.Contracts;

public interface IImportPreviewService
{
    Task<ImportPreviewResponse> PreviewAsync(
        byte[] fileContent,
        ImportFormat format,
        CancellationToken cancellationToken);
}
