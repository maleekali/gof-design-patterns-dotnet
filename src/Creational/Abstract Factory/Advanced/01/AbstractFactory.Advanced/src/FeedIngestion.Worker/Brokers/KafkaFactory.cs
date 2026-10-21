using FeedIngestion.Worker.Abstractions;
using FeedIngestion.Worker.Configuration;
using Microsoft.Extensions.Options;

namespace FeedIngestion.Worker.Brokers;

public sealed class KafkaFactory(
    IBrokerTransport transport,
    IOptions<KafkaOptions> options) : IMessageBrokerFactory
{
    public IMessagePublisher CreatePublisher() => new KafkaMessagePublisher(transport);
    public IBrokerMessageEncoder CreateEventEncoder() => new KafkaEventEncoder(options);
    public IFailurePublisher CreateFailurePublisher() => new KafkaFailurePublisher(transport, options);
}
