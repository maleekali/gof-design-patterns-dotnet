using Xunit;
using System.Text.Json;
using FeedIngestion.Worker.Brokers;
using FeedIngestion.Worker.Configuration;
using FeedIngestion.Worker.Domain;
using Microsoft.Extensions.Options;

namespace FeedIngestion.Tests;

public sealed class KafkaEventEncoderTests
{
    [Fact]
    public void Encode_UsesConfiguredTopicAndPreservesEventPayload()
    {
        var encoder = new KafkaEventEncoder(Options.Create(new KafkaOptions { OrdersTopic = "test.orders" }));
        var input = new OrderImportedEvent(
            "event-1", "ORD-1", "ada@example.com", 12.5m, "USD",
            DateTimeOffset.Parse("2026-01-01T00:00:00Z"), "hash", 3);

        var output = encoder.Encode(input);
        var decoded = JsonSerializer.Deserialize<OrderImportedEvent>(output.Body, JsonDefaults.Options);

        Assert.Equal("test.orders", output.Destination);
        Assert.Equal("ORD-1", output.Key);
        Assert.Equal("event-1", output.MessageId);
        Assert.NotNull(decoded);
        Assert.Equal(input.EventId, decoded.EventId);
        Assert.Equal(input.Amount, decoded.Amount);
    }
}
