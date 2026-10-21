using System.Text.Json;
using ImportPreview.Api.Contracts;

namespace ImportPreview.Api.Reporting;

public sealed class JsonImportReportBuilder : IImportReportBuilder
{
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

    public ErrorReportArtifact Build(ImportValidationResult result)
    {
        var report = new
        {
            result.TotalRows,
            result.ValidRows,
            result.InvalidRows,
            ErrorsReturned = result.Issues.Count,
            Errors = result.Issues
        };

        return new ErrorReportArtifact(
            "import-errors.json",
            "application/json; charset=utf-8",
            JsonSerializer.Serialize(report, Options));
    }
}
