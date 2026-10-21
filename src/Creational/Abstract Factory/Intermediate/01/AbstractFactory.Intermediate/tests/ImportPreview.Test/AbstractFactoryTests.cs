using System.Text;
using ImportPreview.Api.Contracts;
using ImportPreview.Api.Factories;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace ImportPreview.Test;

public sealed class AbstractFactoryTests
{
    [Fact]
    public void CsvFactoryCreatesProductsFromCsvFamily()
    {
        var factory = new CsvImportFactory();

        Assert.IsType<ImportPreview.Api.Parsing.CsvImportParser>(factory.CreateParser());
        Assert.IsType<ImportPreview.Api.Validation.CsvImportValidator>(factory.CreateValidator());
        Assert.IsType<ImportPreview.Api.Reporting.CsvImportReportBuilder>(factory.CreateReportBuilder());
    }

    [Fact]
    public void JsonFactoryCreatesProductsFromJsonFamily()
    {
        var factory = new JsonImportFactory();

        Assert.IsType<ImportPreview.Api.Parsing.JsonImportParser>(factory.CreateParser());
        Assert.IsType<ImportPreview.Api.Validation.JsonImportValidator>(factory.CreateValidator());
        Assert.IsType<ImportPreview.Api.Reporting.JsonImportReportBuilder>(factory.CreateReportBuilder());
    }

    [Fact]
    public async Task CsvPipelineNormalizesRecordsAndFindsInvalidRows()
    {
        const string csv = "id,name,email,amount\n1001,Ada Lovelace,ada@example.com,125.50\n1002,Grace Hopper,not-an-email,40.00\n";
        var factory = new CsvImportFactory();
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));

        var raw = await factory.CreateParser().ParseAsync(stream, CancellationToken.None);
        var validation = factory.CreateValidator().Validate(raw);
        var report = factory.CreateReportBuilder().Build(validation);

        Assert.Equal(2, validation.TotalRows);
        Assert.Equal(1, validation.ValidRows);
        Assert.Equal(1, validation.InvalidRows);
        Assert.Equal("Ada Lovelace", Assert.Single(validation.SampleRecords).Name);
        Assert.Contains(validation.Issues, issue => issue.Field == "email");
        Assert.Equal("text/csv; charset=utf-8", report.ContentType);
        Assert.Contains("email", report.Content);
    }

    [Fact]
    public async Task JsonPipelineNormalizesRecordsAndBuildsJsonErrorReport()
    {
        const string json = """
        [
          { "id": "1001", "name": "Ada Lovelace", "email": "ada@example.com", "amount": 125.50 },
          { "id": "1002", "name": "Grace Hopper", "email": "not-an-email", "amount": 40.00 }
        ]
        """;
        var factory = new JsonImportFactory();
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        var raw = await factory.CreateParser().ParseAsync(stream, CancellationToken.None);
        var validation = factory.CreateValidator().Validate(raw);
        var report = factory.CreateReportBuilder().Build(validation);

        Assert.Equal(2, validation.TotalRows);
        Assert.Equal(1, validation.ValidRows);
        Assert.Equal(1, validation.InvalidRows);
        Assert.Equal("Ada Lovelace", Assert.Single(validation.SampleRecords).Name);
        Assert.Equal("application/json; charset=utf-8", report.ContentType);
        Assert.Contains("email", report.Content);
    }

    [Fact]
    public async Task CsvValidatorReportsMissingRequiredColumns()
    {
        const string csv = "id,name\n1001,Ada Lovelace\n";
        var factory = new CsvImportFactory();
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));

        var raw = await factory.CreateParser().ParseAsync(stream, CancellationToken.None);
        var validation = factory.CreateValidator().Validate(raw);

        Assert.Equal(0, validation.ValidRows);
        Assert.Equal(1, validation.InvalidRows);
        Assert.Contains(validation.Issues, issue => issue.Field == "header");
    }

    [Fact]
    public async Task JsonParserRejectsObjectRootInsteadOfArray()
    {
        const string json = "{ \"id\": \"1001\" }";
        var parser = new JsonImportFactory().CreateParser();
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        await Assert.ThrowsAsync<ImportFormatException>(
            () => parser.ParseAsync(stream, CancellationToken.None));
    }

    [Fact]
    public async Task PreviewServiceReturnsCacheHitForSameFileAndFormat()
    {
        var factories = new IImportFormatFactory[] { new CsvImportFactory(), new JsonImportFactory() };
        var resolver = new ImportFormatFactoryResolver(factories);
        var cache = new TestImportPreviewCache();
        var service = new ImportPreview.Api.Services.ImportPreviewService(
            resolver,
            cache,
            NullLogger<ImportPreview.Api.Services.ImportPreviewService>.Instance);

        var content = Encoding.UTF8.GetBytes("id,name,email,amount\n1001,Ada Lovelace,ada@example.com,125.50\n");
        var first = await service.PreviewAsync(content, ImportFormat.Csv, CancellationToken.None);
        var second = await service.PreviewAsync(content, ImportFormat.Csv, CancellationToken.None);

        Assert.False(first.CacheHit);
        Assert.True(second.CacheHit);
        Assert.Equal(first.ContentHash, second.ContentHash);
        Assert.Equal(first.TotalRows, second.TotalRows);
    }

    private sealed class TestImportPreviewCache : IImportPreviewCache
    {
        private readonly Dictionary<string, ImportPreviewResponse> _values = new();

        public Task<ImportPreviewResponse?> GetAsync(string key, CancellationToken cancellationToken)
        {
            _values.TryGetValue(key, out var value);
            return Task.FromResult(value);
        }

        public Task SetAsync(string key, ImportPreviewResponse value, CancellationToken cancellationToken)
        {
            _values[key] = value;
            return Task.CompletedTask;
        }
    }
}
