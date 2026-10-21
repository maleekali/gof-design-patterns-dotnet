using ImportPreview.Api.Contracts;

namespace ImportPreview.Api.Validation;

public sealed class CsvImportValidator : ImportValidatorBase
{
    private static readonly string[] RequiredColumns = ["id", "name", "email", "amount"];

    protected override IReadOnlyList<ValidationIssue> ValidateDocument(RawImportDocument document)
    {
        var issues = new List<ValidationIssue>();
        foreach (var requiredColumn in RequiredColumns)
        {
            if (!document.Columns.Contains(requiredColumn, StringComparer.OrdinalIgnoreCase))
            {
                issues.Add(new ValidationIssue(1, "header", $"Required CSV column '{requiredColumn}' is missing."));
            }
        }

        return issues;
    }
}
