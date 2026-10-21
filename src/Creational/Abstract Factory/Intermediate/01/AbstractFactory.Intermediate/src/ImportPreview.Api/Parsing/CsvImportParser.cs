using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using ImportPreview.Api.Contracts;

namespace ImportPreview.Api.Parsing;

public sealed class CsvImportParser : IImportParser
{
    public async Task<RawImportDocument> ParseAsync(Stream input, CancellationToken cancellationToken)
    {
        try
        {
            using var reader = new StreamReader(
                input,
                new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true),
                detectEncodingFromByteOrderMarks: true,
                bufferSize: 1024,
                leaveOpen: true);

            var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = args => args.Header.Trim(),
                MissingFieldFound = null
            };

            using var csv = new CsvReader(reader, configuration, leaveOpen: true);

            if (!await csv.ReadAsync())
            {
                throw new ImportFormatException("The CSV file is empty.");
            }

            csv.ReadHeader();
            var originalHeaders = csv.HeaderRecord ?? Array.Empty<string>();
            if (originalHeaders.Length == 0)
            {
                throw new ImportFormatException("The CSV file must contain a header row.");
            }

            var headers = originalHeaders.Select(header => header.Trim()).ToArray();
            if (headers.Any(string.IsNullOrWhiteSpace))
            {
                throw new ImportFormatException("CSV column names cannot be empty.");
            }

            if (headers.Distinct(StringComparer.OrdinalIgnoreCase).Count() != headers.Length)
            {
                throw new ImportFormatException("The CSV file contains duplicate column names.");
            }

            var rows = new List<RawImportRow>();
            var rowNumber = 1; // Header is row 1; data starts at row 2.

            while (await csv.ReadAsync())
            {
                cancellationToken.ThrowIfCancellationRequested();
                rowNumber++;

                var values = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
                for (var index = 0; index < headers.Length; index++)
                {
                    values[headers[index]] = csv.GetField(index);
                }

                rows.Add(new RawImportRow(rowNumber, values));
            }

            return new RawImportDocument(headers, rows);
        }
        catch (ImportFormatException)
        {
            throw;
        }
        catch (CsvHelperException exception)
        {
            throw new ImportFormatException("The CSV file could not be parsed. Check its quoting and delimiter structure.", exception);
        }
        catch (DecoderFallbackException exception)
        {
            throw new ImportFormatException("The CSV file is not valid UTF-8 text.", exception);
        }
    }
}
