using System.Text.Json;

namespace FeedIngestion.Worker.Domain;

public static class JsonDefaults
{
    public static JsonSerializerOptions Options { get; } = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };
}
