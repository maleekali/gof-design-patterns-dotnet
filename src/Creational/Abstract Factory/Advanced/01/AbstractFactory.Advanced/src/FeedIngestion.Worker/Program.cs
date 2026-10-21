using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using FeedIngestion.Worker.Abstractions;
using FeedIngestion.Worker.Brokers;
using FeedIngestion.Worker.Configuration;
using FeedIngestion.Worker.Files;
using FeedIngestion.Worker.Processing;
using FeedIngestion.Worker.State;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<BrokerOptions>(builder.Configuration.GetSection(BrokerOptions.SectionName));
builder.Services.Configure<RedisOptions>(builder.Configuration.GetSection(RedisOptions.SectionName));
builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection(RabbitMqOptions.SectionName));
builder.Services.Configure<KafkaOptions>(builder.Configuration.GetSection(KafkaOptions.SectionName));
builder.Services.Configure<FeedOptions>(builder.Configuration.GetSection(FeedOptions.SectionName));
builder.Services.Configure<FileSourceOptions>(builder.Configuration.GetSection(FileSourceOptions.SectionName));
builder.Services.Configure<SftpOptions>(builder.Configuration.GetSection(SftpOptions.SectionName));

builder.Services.AddSingleton<IConnectionMultiplexer>(serviceProvider =>
{
    var options = serviceProvider.GetRequiredService<IOptions<RedisOptions>>().Value;
    return ConnectionMultiplexer.Connect(options.ConnectionString);
});
builder.Services.AddSingleton<IProcessingStateStore, RedisProcessingStateStore>();
var fileSourceProvider = builder.Configuration[$"{FileSourceOptions.SectionName}:Provider"] ?? "Directory";
switch (fileSourceProvider.Trim().ToLowerInvariant())
{
    case "directory":
        builder.Services.AddSingleton<IExternalFileSource, DirectoryFileSource>();
        break;
    case "sftp":
        builder.Services.AddSingleton<IExternalFileSource, SftpExternalFileSource>();
        break;
    default:
        throw new InvalidOperationException(
            $"Unsupported file source '{fileSourceProvider}'. Supported values: Directory, Sftp.");
}

builder.Services.AddSingleton<IOrderFeedRecordParser, OrderFeedRecordParser>();

var brokerProvider = builder.Configuration[$"{BrokerOptions.SectionName}:Provider"] ?? "RabbitMQ";
switch (brokerProvider.Trim().ToLowerInvariant())
{
    case "rabbitmq":
        builder.Services.AddSingleton<IBrokerTransport, RabbitMqTransport>();
        builder.Services.AddSingleton<IMessageBrokerFactory, RabbitMqFactory>();
        break;
    case "kafka":
        builder.Services.AddSingleton<IBrokerTransport, KafkaTransport>();
        builder.Services.AddSingleton<IMessageBrokerFactory, KafkaFactory>();
        break;
    default:
        throw new InvalidOperationException(
            $"Unsupported broker provider '{brokerProvider}'. Supported values: RabbitMQ, Kafka.");
}

builder.Services.AddHostedService<FeedIngestionWorker>();

await builder.Build().RunAsync();
