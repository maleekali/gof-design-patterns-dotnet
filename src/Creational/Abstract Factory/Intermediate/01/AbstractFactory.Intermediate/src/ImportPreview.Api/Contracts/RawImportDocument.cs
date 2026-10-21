namespace ImportPreview.Api.Contracts;

public sealed record RawImportRow(
    int RowNumber,
    IReadOnlyDictionary<string, string?> Values,
    bool IsObject = true);

public sealed record RawImportDocument(
    IReadOnlyList<string> Columns,
    IReadOnlyList<RawImportRow> Rows);
