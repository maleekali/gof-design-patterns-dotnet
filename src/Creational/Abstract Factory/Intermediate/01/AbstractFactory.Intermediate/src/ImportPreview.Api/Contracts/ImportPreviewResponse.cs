namespace ImportPreview.Api.Contracts;

public sealed record ErrorReportArtifact(
    string FileName,
    string ContentType,
    string Content);

public sealed record ImportPreviewResponse(
    string Format,
    string ContentHash,
    int TotalRows,
    int ValidRows,
    int InvalidRows,
    IReadOnlyList<ImportRecord> SampleRecords,
    IReadOnlyList<ValidationIssue> Issues,
    string ErrorReportFileName,
    string ErrorReportContentType,
    string ErrorReportContent,
    bool CacheHit,
    DateTimeOffset GeneratedAtUtc);
