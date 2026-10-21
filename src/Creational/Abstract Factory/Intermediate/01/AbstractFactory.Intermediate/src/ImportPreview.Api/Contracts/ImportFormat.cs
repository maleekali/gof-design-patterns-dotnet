namespace ImportPreview.Api.Contracts;

public enum ImportFormat
{
    Csv,
    Json
}

public static class ImportFormatExtensions
{
    public static bool TryParse(string? value, out ImportFormat format)
    {
        if (string.Equals(value, "csv", StringComparison.OrdinalIgnoreCase))
        {
            format = ImportFormat.Csv;
            return true;
        }

        if (string.Equals(value, "json", StringComparison.OrdinalIgnoreCase))
        {
            format = ImportFormat.Json;
            return true;
        }

        format = default;
        return false;
    }

    public static string ToKey(this ImportFormat format) => format.ToString().ToLowerInvariant();
}
