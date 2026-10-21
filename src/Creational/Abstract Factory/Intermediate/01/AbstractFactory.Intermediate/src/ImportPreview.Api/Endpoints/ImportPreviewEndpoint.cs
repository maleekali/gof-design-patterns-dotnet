using ImportPreview.Api.Contracts;

namespace ImportPreview.Api.Endpoints;

public static class ImportPreviewEndpoint
{
    private const long MaxFileBytes = 5 * 1024 * 1024;

    public static async Task<IResult> HandleAsync(
        HttpRequest request,
        IImportPreviewService service,
        CancellationToken cancellationToken)
    {
        if (!request.HasFormContentType)
        {
            return Results.BadRequest(new { error = "Expected multipart/form-data with a 'file' field." });
        }

        IFormCollection form;
        try
        {
            form = await request.ReadFormAsync(cancellationToken);
        }
        catch (InvalidDataException)
        {
            return Results.BadRequest(new { error = "The multipart form is invalid or exceeds the configured upload limit." });
        }

        var file = form.Files.GetFile("file");
        if (file is null)
        {
            return Results.BadRequest(new { error = "The form must contain a file field named 'file'." });
        }

        if (file.Length == 0)
        {
            return Results.BadRequest(new { error = "The uploaded file is empty." });
        }

        if (file.Length > MaxFileBytes)
        {
            return Results.BadRequest(new { error = "The maximum file size is 5 MiB." });
        }

        var requestedFormat = form["format"].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(requestedFormat))
        {
            requestedFormat = Path.GetExtension(file.FileName).TrimStart('.');
        }

        if (!ImportFormatExtensions.TryParse(requestedFormat, out var format))
        {
            return Results.BadRequest(new { error = "Unsupported format. Use 'csv' or 'json'." });
        }

        await using var buffer = new MemoryStream();
        await file.CopyToAsync(buffer, cancellationToken);

        try
        {
            var result = await service.PreviewAsync(buffer.ToArray(), format, cancellationToken);
            return Results.Ok(result);
        }
        catch (ImportFormatException exception)
        {
            return Results.BadRequest(new { error = exception.Message });
        }
    }
}
