namespace ImportPreview.Api.Contracts;

public interface IImportFormatFactory
{
    ImportFormat Format { get; }
    IImportParser CreateParser();
    IImportValidator CreateValidator();
    IImportReportBuilder CreateReportBuilder();
}

public interface IImportFormatFactoryResolver
{
    IImportFormatFactory Resolve(ImportFormat format);
}

public interface IImportParser
{
    Task<RawImportDocument> ParseAsync(Stream input, CancellationToken cancellationToken);
}

public interface IImportValidator
{
    ImportValidationResult Validate(RawImportDocument document);
}

public interface IImportReportBuilder
{
    ErrorReportArtifact Build(ImportValidationResult result);
}
