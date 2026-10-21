using ImportPreview.Api.Contracts;

namespace ImportPreview.Api.Validation;

// JSON has no shared header row. The base validator checks required properties on every object.
public sealed class JsonImportValidator : ImportValidatorBase
{
}
