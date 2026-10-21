using ImportPreview.Api.Contracts;

namespace ImportPreview.Api.Factories;

public sealed class ImportFormatFactoryResolver : IImportFormatFactoryResolver
{
    private readonly IReadOnlyDictionary<ImportFormat, IImportFormatFactory> _factories;

    public ImportFormatFactoryResolver(IEnumerable<IImportFormatFactory> factories)
    {
        var factoryList = factories.ToArray();
        var duplicate = factoryList
            .GroupBy(factory => factory.Format)
            .FirstOrDefault(group => group.Count() > 1);

        if (duplicate is not null)
        {
            throw new InvalidOperationException(
                $"More than one import factory is registered for {duplicate.Key}.");
        }

        _factories = factoryList.ToDictionary(factory => factory.Format);
    }

    public IImportFormatFactory Resolve(ImportFormat format)
    {
        if (_factories.TryGetValue(format, out var factory))
        {
            return factory;
        }

        throw new ImportFormatException($"Import format '{format}' is not supported.");
    }
}
