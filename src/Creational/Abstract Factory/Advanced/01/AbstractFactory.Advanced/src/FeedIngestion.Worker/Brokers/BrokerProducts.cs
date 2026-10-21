using System.Text.Json;
using FeedIngestion.Worker.Abstractions;
using FeedIngestion.Worker.Configuration;
using FeedIngestion.Worker.Domain;
using Microsoft.Extensions.Options;

namespace FeedIngestion.Worker.Brokers;

public sealed class RabbitMqMessagePublisher(IBrokerTransport transport) : IMessagePublisher
{
    public Task PublishAsync(OutboundMessage message, CancellationToken cancellationToken)
        => transport.PublishAsync(message, cancellationToken);
}

public sealed class KafkaMessagePublisher(IBrokerTransport transport) : IMessagePublisher
{
    public Task PublishAsync(OutboundMessage message, CancellationToken cancellationToken)
        => transport.PublishAsync(message, cancellationToken);
}

public sealed class RabbitMqEventEncoder(IOptions<RabbitMqOptions> options) : IBrokerMessageEncoder
{
    public OutboundMessage Encode(OrderImportedEvent message)
        => new(
            options.Value.OrdersRoutingKey,
            message.OrderId,
            message.EventId,
            JsonSerializer.SerializeToUtf8Bytes(message, JsonDefaults.Options));
}

public sealed class KafkaEventEncoder(IOptions<KafkaOptions> options) : IBrokerMessageEncoder
{
    public OutboundMessage Encode(OrderImportedEvent message)
        => new(
            options.Value.OrdersTopic,
            message.OrderId,
            message.EventId,
            JsonSerializer.SerializeToUtf8Bytes(message, JsonDefaults.Options));
}

public sealed class RabbitMqFailurePublisher(
    IBrokerTransport transport,
    IOptions<RabbitMqOptions> options) : IFailurePublisher
{
    public Task PublishAsync(FeedFailure failure, CancellationToken cancellationToken)
    {
        var message = new OutboundMessage(
            options.Value.FailureRoutingKey,
            failure.EventId,
            failure.EventId,
            JsonSerializer.SerializeToUtf8Bytes(failure, JsonDefaults.Options));
        return transport.PublishAsync(message, cancellationToken);
    }
}

public sealed class KafkaFailurePublisher(
    IBrokerTransport transport,
    IOptions<KafkaOptions> options) : IFailurePublisher
{
    public Task PublishAsync(FeedFailure failure, CancellationToken cancellationToken)
    {
        var message = new OutboundMessage(
            options.Value.FailureTopic,
            failure.EventId,
            failure.EventId,
            JsonSerializer.SerializeToUtf8Bytes(failure, JsonDefaults.Options));
        return transport.PublishAsync(message, cancellationToken);
    }
}
