using System.Text.Json;
using FeedIngestion.Worker.Abstractions;
using FeedIngestion.Worker.Domain;

namespace FeedIngestion.Worker.Processing;

public sealed class OrderFeedRecordParser : IOrderFeedRecordParser
{
    public bool TryParse(string jsonLine, out OrderFeedRecord? record, out string error)
    {
        record = null;
        error = string.Empty;

        OrderFeedRecord? parsed;
        try
        {
            parsed = JsonSerializer.Deserialize<OrderFeedRecord>(jsonLine, JsonDefaults.Options);
        }
        catch (JsonException)
        {
            error = "Line is not valid JSON.";
            return false;
        }

        if (parsed is null)
        {
            error = "Line does not contain a record.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(parsed.OrderId))
        {
            error = "OrderId is required.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(parsed.CustomerEmail) || !parsed.CustomerEmail.Contains('@'))
        {
            error = "CustomerEmail must contain '@'.";
            return false;
        }

        if (parsed.Amount <= 0)
        {
            error = "Amount must be greater than zero.";
            return false;
        }

        if (parsed.Currency is null || parsed.Currency.Length != 3 || !parsed.Currency.All(char.IsAsciiLetter))
        {
            error = "Currency must be a three-letter code.";
            return false;
        }

        record = new OrderFeedRecord
        {
            OrderId = parsed.OrderId.Trim(),
            CustomerEmail = parsed.CustomerEmail.Trim(),
            Amount = parsed.Amount,
            Currency = parsed.Currency.ToUpperInvariant()
        };

        return true;
    }
}
