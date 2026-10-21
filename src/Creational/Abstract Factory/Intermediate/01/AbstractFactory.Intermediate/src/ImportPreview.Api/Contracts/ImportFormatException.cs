namespace ImportPreview.Api.Contracts;

public sealed class ImportFormatException : Exception
{
    public ImportFormatException(string message) : base(message) { }

    public ImportFormatException(string message, Exception innerException)
        : base(message, innerException) { }
}
