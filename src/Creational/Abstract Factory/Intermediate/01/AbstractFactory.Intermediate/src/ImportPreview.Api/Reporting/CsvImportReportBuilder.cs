using System.Text;
using ImportPreview.Api.Contracts;

namespace ImportPreview.Api.Reporting;

public sealed class CsvImportReportBuilder : IImportReportBuilder
{
    public ErrorReportArtifact Build(ImportValidationResult result)
    {
        var builder = new StringBuilder();
        builder.AppendLine("row,field,message");

        foreach (var issue in result.Issues)
        {
            builder.Append(issue.RowNumber).Append(',')
                .Append(Escape(issue.Field)).Append(',')
                .Append(Escape(issue.Message)).AppendLine();
        }

        return new ErrorReportArtifact(
            "import-errors.csv",
            "text/csv; charset=utf-8",
            builder.ToString());
    }

    private static string Escape(string value)
    {
        if (!value.Contains(',') && !value.Contains('"') && !value.Contains('\r') && !value.Contains('\n'))
        {
            return value;
        }

        var escaped = value.Replace("\"", "\"\"");
        return "\"" + escaped + "\"";
    }
}
