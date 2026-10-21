using System.Text;
using System.Text.Json;
using ImportPreview.Api.Contracts;

namespace ImportPreview.Api.Parsing;

public sealed class JsonImportParser : IImportParser
{
    public async Task<RawImportDocument> ParseAsync(Stream input, CancellationToken cancellationToken)
    {
        try
        {
            using var document = await JsonDocument.ParseAsync(
                input,
                new JsonDocumentOptions { MaxDepth = 32, CommentHandling = JsonCommentHandling.Disallow },
                cancellationToken);

            if (document.RootElement.ValueKind != JsonValueKind.Array)
            {
                throw new ImportFormatException("A JSON import must be an array of objects.");
            }

            var rows = new List<RawImportRow>();
            var columns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var rowNumber = 0;

            foreach (var element in document.RootElement.EnumerateArray())
            {
                cancellationToken.ThrowIfCancellationRequested();
                rowNumber++;

                if (element.ValueKind != JsonValueKind.Object)
                {
                    rows.Add(new RawImportRow(rowNumber, new Dictionary<string, string?>(), IsObject: false));
                    continue;
                }

                var values = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
                foreach (var property in element.EnumerateObject())
                {
                    columns.Add(property.Name);
                    values[property.Name] = ConvertValue(property.Value);
                }

                rows.Add(new RawImportRow(rowNumber, values));
            }

            return new RawImportDocument(columns.OrderBy(value => value, StringComparer.OrdinalIgnoreCase).ToArray(), rows);
        }
        catch (ImportFormatException)
        {
            throw;
        }
        catch (JsonException exception)
        {
            throw new ImportFormatException("The JSON file is malformed or exceeds the supported nesting depth.", exception);
        }
        catch (DecoderFallbackException exception)
        {
            throw new ImportFormatException("The JSON file is not valid UTF-8 text.", exception);
        }
    }

    private static string? ConvertValue(JsonElement value) => value.ValueKind switch
    {
        JsonValueKind.String => value.GetString(),
        JsonValueKind.Number => value.GetRawText(),
        JsonValueKind.True => "true",
        JsonValueKind.False => "false",
        JsonValueKind.Null => null,
        _ => value.GetRawText()
    };
}
