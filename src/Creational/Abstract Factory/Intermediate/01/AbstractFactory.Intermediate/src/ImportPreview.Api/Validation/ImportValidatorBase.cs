using System.Globalization;
using System.Text.RegularExpressions;
using ImportPreview.Api.Contracts;

namespace ImportPreview.Api.Validation;

public abstract class ImportValidatorBase : IImportValidator
{
    private const int MaxIssues = 100;
    private const int MaxSampleRecords = 10;
    private static readonly Regex EmailPattern = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public ImportValidationResult Validate(RawImportDocument document)
    {
        var documentIssues = ValidateDocument(document);
        if (documentIssues.Count > 0)
        {
            return new ImportValidationResult(
                document.Rows.Count,
                ValidRows: 0,
                InvalidRows: document.Rows.Count,
                SampleRecords: Array.Empty<ImportRecord>(),
                Issues: documentIssues.Take(MaxIssues).ToArray());
        }

        var validRows = 0;
        var invalidRows = 0;
        var samples = new List<ImportRecord>(MaxSampleRecords);
        var issues = new List<ValidationIssue>(MaxIssues);

        foreach (var row in document.Rows)
        {
            var rowIssues = ValidateRow(row, out var record);
            if (rowIssues.Count > 0)
            {
                invalidRows++;
                foreach (var issue in rowIssues)
                {
                    if (issues.Count == MaxIssues)
                    {
                        break;
                    }
                    issues.Add(issue);
                }
                continue;
            }

            validRows++;
            if (record is not null && samples.Count < MaxSampleRecords)
            {
                samples.Add(record);
            }
        }

        return new ImportValidationResult(
            document.Rows.Count,
            validRows,
            invalidRows,
            samples,
            issues);
    }

    protected virtual IReadOnlyList<ValidationIssue> ValidateDocument(RawImportDocument document)
        => Array.Empty<ValidationIssue>();

    private static List<ValidationIssue> ValidateRow(RawImportRow row, out ImportRecord? record)
    {
        var issues = new List<ValidationIssue>();
        record = null;

        if (!row.IsObject)
        {
            issues.Add(new ValidationIssue(row.RowNumber, "$", "Each JSON array item must be an object."));
            return issues;
        }

        string? Get(string name) => row.Values.TryGetValue(name, out var value) ? value?.Trim() : null;

        var id = Get("id");
        var name = Get("name");
        var email = Get("email");
        var amountText = Get("amount");

        if (string.IsNullOrWhiteSpace(id))
            issues.Add(new ValidationIssue(row.RowNumber, "id", "The id field is required."));

        if (string.IsNullOrWhiteSpace(name))
            issues.Add(new ValidationIssue(row.RowNumber, "name", "The name field is required."));

        if (string.IsNullOrWhiteSpace(email))
            issues.Add(new ValidationIssue(row.RowNumber, "email", "The email field is required."));
        else if (!EmailPattern.IsMatch(email))
            issues.Add(new ValidationIssue(row.RowNumber, "email", "The email field must have a basic email format."));

        decimal amount = 0;
        if (string.IsNullOrWhiteSpace(amountText))
        {
            issues.Add(new ValidationIssue(row.RowNumber, "amount", "The amount field is required."));
        }
        else if (!decimal.TryParse(amountText, NumberStyles.Number, CultureInfo.InvariantCulture, out amount))
        {
            issues.Add(new ValidationIssue(row.RowNumber, "amount", "The amount must be a decimal using invariant-culture notation."));
        }
        else if (amount < 0)
        {
            issues.Add(new ValidationIssue(row.RowNumber, "amount", "The amount cannot be negative."));
        }

        if (issues.Count == 0)
        {
            record = new ImportRecord(id!, name!, email!, amount, row.RowNumber);
        }

        return issues;
    }
}
