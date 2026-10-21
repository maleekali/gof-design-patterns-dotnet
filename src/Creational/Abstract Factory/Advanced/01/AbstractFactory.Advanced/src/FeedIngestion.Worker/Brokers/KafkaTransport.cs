using FeedIngestion.Worker.Abstractions;
using Confluent.Kafka;
using FeedIngestion.Worker.Configuration;
using FeedIngestion.Worker.Domain;
using Microsoft.Extensions.Options;

namespace FeedIngestion.Worker.Brokers;

public sealed class KafkaTransport : IBrokerTransport
{
    private readonly IProducer<string, byte[]> _producer;

    public KafkaTransport(IOptions<KafkaOptions> options)
    {
        var settings = options.Value;
        _producer = new ProducerBuilder<string, byte[]>(new ProducerConfig
        {
            BootstrapServers = settings.BootstrapServers,
            ClientId = settings.ClientId,
            EnableIdempotence = true,
            Acks = Acks.All
        }).Build();
    }

    public async Task PublishAsync(OutboundMessage message, CancellationToken cancellationToken)
    {
        var result = await _producer.ProduceAsync(
            message.Destination,
            new Message<string, byte[]> { Key = message.Key, Value = message.Body },
            cancellationToken);

        if (result.Status == PersistenceStatus.NotPersisted)
            throw new InvalidOperationException($"Kafka did not persist the message to '{message.Destination}'.");
    }

    public ValueTask DisposeAsync()
    {
        _producer.Flush(TimeSpan.FromSeconds(10));
        _producer.Dispose();
        return ValueTask.CompletedTask;
    }
}
