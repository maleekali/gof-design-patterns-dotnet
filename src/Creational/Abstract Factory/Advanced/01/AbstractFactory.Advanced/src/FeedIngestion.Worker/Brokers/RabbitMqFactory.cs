using FeedIngestion.Worker.Abstractions;
using FeedIngestion.Worker.Configuration;
using Microsoft.Extensions.Options;

namespace FeedIngestion.Worker.Brokers;

public sealed class RabbitMqFactory(
    IBrokerTransport transport,
    IOptions<RabbitMqOptions> options) : IMessageBrokerFactory
{
    public IMessagePublisher CreatePublisher() => new RabbitMqMessagePublisher(transport);
    public IBrokerMessageEncoder CreateEventEncoder() => new RabbitMqEventEncoder(options);
    public IFailurePublisher CreateFailurePublisher() => new RabbitMqFailurePublisher(transport, options);
}
