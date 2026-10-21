using ImportPreview.Api.Contracts;
using ImportPreview.Api.Parsing;
using ImportPreview.Api.Reporting;
using ImportPreview.Api.Validation;

namespace ImportPreview.Api.Factories;

public sealed class CsvImportFactory : IImportFormatFactory
{
    public ImportFormat Format => ImportFormat.Csv;
    public IImportParser CreateParser() => new CsvImportParser();
    public IImportValidator CreateValidator() => new CsvImportValidator();
    public IImportReportBuilder CreateReportBuilder() => new CsvImportReportBuilder();
}
