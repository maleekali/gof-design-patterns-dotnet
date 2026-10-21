using Xunit;
using FeedIngestion.Worker.Processing;

namespace FeedIngestion.Tests;

public sealed class OrderFeedRecordParserTests
{
    private readonly OrderFeedRecordParser _parser = new();

    [Fact]
    public void TryParse_ValidRecord_ReturnsNormalizedRecord()
    {
        const string json = "{\"orderId\":\" ORD-1 \",\"customerEmail\":\"ada@example.com\",\"amount\":12.5,\"currency\":\"usd\"}";

        var result = _parser.TryParse(json, out var record, out var error);

        Assert.True(result);
        Assert.Empty(error);
        Assert.NotNull(record);
        Assert.Equal("ORD-1", record.OrderId);
        Assert.Equal("USD", record.Currency);
    }

    [Theory]
    [InlineData("not-json", "Line is not valid JSON.")]
    [InlineData("{}", "OrderId is required.")]
    [InlineData("{\"orderId\":\"A\",\"customerEmail\":\"a@example.com\",\"amount\":0,\"currency\":\"USD\"}", "Amount must be greater than zero.")]
    [InlineData("{\"orderId\":\"A\",\"customerEmail\":\"a@example.com\",\"amount\":1,\"currency\":\"US\"}", "Currency must be a three-letter code.")]
    public void TryParse_InvalidRecord_ReturnsReason(string json, string expectedError)
    {
        var result = _parser.TryParse(json, out var record, out var error);

        Assert.False(result);
        Assert.Null(record);
        Assert.Equal(expectedError, error);
    }
}
