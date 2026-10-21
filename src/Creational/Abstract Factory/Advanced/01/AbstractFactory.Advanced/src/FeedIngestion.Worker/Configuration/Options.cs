namespace FeedIngestion.Worker.Configuration;

public sealed class BrokerOptions
{
    public const string SectionName = "Broker";
    public string Provider { get; set; } = "RabbitMQ";
}

public sealed class RedisOptions
{
    public const string SectionName = "Redis";
    public string ConnectionString { get; set; } = "localhost:6379";
}

public sealed class RabbitMqOptions
{
    public const string SectionName = "RabbitMq";
    public string HostName { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string Exchange { get; set; } = "feed.events";
    public string OrdersRoutingKey { get; set; } = "orders.imported";
    public string OrdersQueue { get; set; } = "feed.orders";
    public string FailureRoutingKey { get; set; } = "feed.failures";
    public string FailureQueue { get; set; } = "feed.failures";
}

public sealed class KafkaOptions
{
    public const string SectionName = "Kafka";
    public string BootstrapServers { get; set; } = "localhost:9092";
    public string ClientId { get; set; } = "feed-ingestion-worker";
    public string OrdersTopic { get; set; } = "feed.order-imported";
    public string FailureTopic { get; set; } = "feed.failures";
}

public sealed class FeedOptions
{
    public const string SectionName = "Feed";
    public string IncomingDirectory { get; set; } = "data/incoming";
    public string ProcessedDirectory { get; set; } = "data/processed";
    public int PollIntervalSeconds { get; set; } = 5;
    public int LeaseSeconds { get; set; } = 300;
    public int RecordMarkerDays { get; set; } = 30;
    public int CompletedFileDays { get; set; } = 30;
}

public sealed class FileSourceOptions
{
    public const string SectionName = "FileSource";
    public string Provider { get; set; } = "Directory";
}

public sealed class SftpOptions
{
    public const string SectionName = "Sftp";
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 22;
    public string UserName { get; set; } = "feeduser";
    public string Password { get; set; } = "change-me";
    public string IncomingDirectory { get; set; } = "/home/feeduser/upload";
    public string ProcessedDirectory { get; set; } = "/home/feeduser/processed";
    public string LocalSpoolDirectory { get; set; } = "data/sftp-spool";
}
