namespace ImportPreview.Api.Contracts;

public sealed record ValidationIssue(int RowNumber, string Field, string Message);

public sealed record ImportValidationResult(
    int TotalRows,
    int ValidRows,
    int InvalidRows,
    IReadOnlyList<ImportRecord> SampleRecords,
    IReadOnlyList<ValidationIssue> Issues);
