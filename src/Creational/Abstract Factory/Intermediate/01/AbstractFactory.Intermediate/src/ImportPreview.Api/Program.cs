using ImportPreview.Api.Caching;
using ImportPreview.Api.Contracts;
using ImportPreview.Api.Endpoints;
using ImportPreview.Api.Factories;
using ImportPreview.Api.Services;
using Microsoft.AspNetCore.Http.Features;
using StackExchange.Redis;

const long maxRequestBytes = 6 * 1024 * 1024;
const long maxFileBytes = 5 * 1024 * 1024;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = maxRequestBytes;
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = maxRequestBytes;
});

builder.Services.AddSingleton<IImportFormatFactory, CsvImportFactory>();
builder.Services.AddSingleton<IImportFormatFactory, JsonImportFactory>();
builder.Services.AddSingleton<IImportFormatFactoryResolver, ImportFormatFactoryResolver>();
builder.Services.AddScoped<IImportPreviewService, ImportPreviewService>();

var redisConnectionString = builder.Configuration.GetConnectionString("Redis")
    ?? "localhost:6379,password=local-dev-password,abortConnect=false";

builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
{
    var options = ConfigurationOptions.Parse(redisConnectionString);
    options.AbortOnConnectFail = false;
    options.ConnectRetry = 1;
    options.ConnectTimeout = 1500;
    options.AsyncTimeout = 1500;
    return ConnectionMultiplexer.Connect(options);
});

builder.Services.AddSingleton<IImportPreviewCache>(serviceProvider =>
{
    var connection = serviceProvider.GetRequiredService<IConnectionMultiplexer>();
    var logger = serviceProvider.GetRequiredService<ILogger<RedisImportPreviewCache>>();
    var ttlMinutes = builder.Configuration.GetValue<int?>("PreviewCache:TimeToLiveMinutes") ?? 10;
    return new RedisImportPreviewCache(connection, logger, TimeSpan.FromMinutes(Math.Clamp(ttlMinutes, 1, 120)));
});

var app = builder.Build();

app.MapGet("/", () => Results.Ok(new
{
    service = "Import Preview Service",
    pattern = "Abstract Factory",
    endpoints = new[] { "GET /health", "POST /imports/preview" }
}));

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));
app.MapPost("/imports/preview", ImportPreviewEndpoint.HandleAsync)
    .WithName("PreviewImport");

app.Run();

public partial class Program { }
