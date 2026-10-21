using FeedIngestion.Worker.Abstractions;
using FeedIngestion.Worker.Configuration;
using FeedIngestion.Worker.Domain;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace FeedIngestion.Worker.Brokers;

/// <summary>One long-lived connection/channel; publishes are serialized because an IChannel is not thread-safe.</summary>
public sealed class RabbitMqTransport : IBrokerTransport
{
    private readonly RabbitMqOptions _options;
    private readonly SemaphoreSlim _publishGate = new(1, 1);
    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMqTransport(IOptions<RabbitMqOptions> options) => _options = options.Value;

    public async Task PublishAsync(OutboundMessage message, CancellationToken cancellationToken)
    {
        await _publishGate.WaitAsync(cancellationToken);
        try
        {
            await EnsureConnectedAsync(cancellationToken);
            var properties = new BasicProperties
            {
                ContentType = message.ContentType,
                MessageId = message.MessageId,
                Persistent = true
            };

            await _channel!.BasicPublishAsync(
                exchange: _options.Exchange,
                routingKey: message.Destination,
                mandatory: true,
                basicProperties: properties,
                body: message.Body,
                cancellationToken: cancellationToken);
        }
        finally
        {
            _publishGate.Release();
        }
    }

    private async Task EnsureConnectedAsync(CancellationToken cancellationToken)
    {
        if (_connection?.IsOpen == true && _channel?.IsOpen == true)
            return;

        if (_channel is not null)
        {
            await _channel.DisposeAsync();
            _channel = null;
        }

        if (_connection is null || !_connection.IsOpen)
        {
            if (_connection is not null)
                await _connection.DisposeAsync();

            var factory = new ConnectionFactory
            {
                HostName = _options.HostName,
                Port = _options.Port,
                UserName = _options.UserName,
                Password = _options.Password,
                AutomaticRecoveryEnabled = true,
                TopologyRecoveryEnabled = true
            };
            _connection = await factory.CreateConnectionAsync("gof-feed-ingestion", cancellationToken);
        }

        var channelOptions = new CreateChannelOptions(
            publisherConfirmationsEnabled: true,
            publisherConfirmationTrackingEnabled: true);
        _channel = await _connection.CreateChannelAsync(channelOptions, cancellationToken);
        await _channel.ExchangeDeclareAsync(
            exchange: _options.Exchange,
            type: ExchangeType.Topic,
            durable: true,
            cancellationToken: cancellationToken);
        await _channel.QueueDeclareAsync(
            queue: _options.OrdersQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken);
        await _channel.QueueBindAsync(
            queue: _options.OrdersQueue,
            exchange: _options.Exchange,
            routingKey: _options.OrdersRoutingKey,
            cancellationToken: cancellationToken);
        await _channel.QueueDeclareAsync(
            queue: _options.FailureQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken);
        await _channel.QueueBindAsync(
            queue: _options.FailureQueue,
            exchange: _options.Exchange,
            routingKey: _options.FailureRoutingKey,
            cancellationToken: cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null)
            await _channel.DisposeAsync();
        if (_connection is not null)
            await _connection.DisposeAsync();
        _publishGate.Dispose();
    }
}
