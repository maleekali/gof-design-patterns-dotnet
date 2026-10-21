using System.Security.Cryptography;
using ImportPreview.Api.Contracts;

namespace ImportPreview.Api.Services;

public sealed class ImportPreviewService : IImportPreviewService
{
    private const string CacheSchemaVersion = "v1";
    private readonly IImportFormatFactoryResolver _factoryResolver;
    private readonly IImportPreviewCache _cache;
    private readonly ILogger<ImportPreviewService> _logger;

    public ImportPreviewService(
        IImportFormatFactoryResolver factoryResolver,
        IImportPreviewCache cache,
        ILogger<ImportPreviewService> logger)
    {
        _factoryResolver = factoryResolver;
        _cache = cache;
        _logger = logger;
    }

    public async Task<ImportPreviewResponse> PreviewAsync(
        byte[] fileContent,
        ImportFormat format,
        CancellationToken cancellationToken)
    {
        if (fileContent.Length == 0)
        {
            throw new ImportFormatException("The uploaded file is empty.");
        }

        cancellationToken.ThrowIfCancellationRequested();
        var contentHash = Convert.ToHexString(SHA256.HashData(fileContent)).ToLowerInvariant();
        var cacheKey = $"import-preview:{CacheSchemaVersion}:{format.ToKey()}:{contentHash}";

        var cached = await _cache.GetAsync(cacheKey, cancellationToken);
        if (cached is not null)
        {
            _logger.LogInformation("Import preview cache hit for {Format} content {ContentHash}.", format, contentHash);
            return cached with { CacheHit = true };
        }

        var factory = _factoryResolver.Resolve(format);
        using var stream = new MemoryStream(fileContent, writable: false);
        var rawDocument = await factory.CreateParser().ParseAsync(stream, cancellationToken);
        var validation = factory.CreateValidator().Validate(rawDocument);
        var report = factory.CreateReportBuilder().Build(validation);

        var response = new ImportPreviewResponse(
            Format: format.ToKey(),
            ContentHash: contentHash,
            TotalRows: validation.TotalRows,
            ValidRows: validation.ValidRows,
            InvalidRows: validation.InvalidRows,
            SampleRecords: validation.SampleRecords,
            Issues: validation.Issues,
            ErrorReportFileName: report.FileName,
            ErrorReportContentType: report.ContentType,
            ErrorReportContent: report.Content,
            CacheHit: false,
            GeneratedAtUtc: DateTimeOffset.UtcNow);

        await _cache.SetAsync(cacheKey, response, cancellationToken);
        _logger.LogInformation(
            "Processed {Format} import {ContentHash}: {ValidRows} valid and {InvalidRows} invalid rows.",
            format, contentHash, validation.ValidRows, validation.InvalidRows);

        return response;
    }
}
