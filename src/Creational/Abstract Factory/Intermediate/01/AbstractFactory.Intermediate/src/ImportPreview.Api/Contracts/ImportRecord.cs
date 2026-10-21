namespace ImportPreview.Api.Contracts;

public sealed record ImportRecord(
    string Id,
    string Name,
    string Email,
    decimal Amount,
    int SourceRow);
