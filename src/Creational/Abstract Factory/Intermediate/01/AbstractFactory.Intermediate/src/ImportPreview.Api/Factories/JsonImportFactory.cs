using ImportPreview.Api.Contracts;
using ImportPreview.Api.Parsing;
using ImportPreview.Api.Reporting;
using ImportPreview.Api.Validation;

namespace ImportPreview.Api.Factories;

public sealed class JsonImportFactory : IImportFormatFactory
{
    public ImportFormat Format => ImportFormat.Json;
    public IImportParser CreateParser() => new JsonImportParser();
    public IImportValidator CreateValidator() => new JsonImportValidator();
    public IImportReportBuilder CreateReportBuilder() => new JsonImportReportBuilder();
}
