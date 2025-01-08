using Microsoft.Azure.Cosmos;
using ReactWithASP.Server.Services;
using Microsoft.Extensions.Logging;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;

//Config: To run locally, make sure Multiple startup projects is selected under solution properties, with action Start for both

var builder = WebApplication.CreateBuilder(args);

// Configure logging based on environment
if (builder.Environment.IsDevelopment())
{
    // Development logging configuration
    builder.Logging
        .ClearProviders()
        .AddDebug()
        .AddConsole()
        .SetMinimumLevel(LogLevel.Debug)
        .AddFilter("Microsoft.AspNetCore", LogLevel.Warning)
        .AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning)
        .AddFilter("ReactWithASP.Server", LogLevel.Debug);

    var loggerFactory = LoggerFactory.Create(builder =>
    {
        builder
            .AddConsole()
            .AddDebug()
            .SetMinimumLevel(LogLevel.Information);
    });

    var logger = loggerFactory.CreateLogger<Program>();
    logger.LogInformation("=== Environment Information ===");
    logger.LogInformation("Current environment: {Environment}", builder.Environment.EnvironmentName);
    logger.LogInformation("Content Root Path: {ContentRootPath}", builder.Environment.ContentRootPath);
}
else
{
    // Production logging with Azure Monitor Application Insights OpenTelemetry
    builder.Services.AddOpenTelemetry()
        .UseAzureMonitor()
        .WithTracing(tracing =>
        {
            tracing
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation();
        })
        .WithMetrics(metrics =>
        {
            metrics.AddAspNetCoreInstrumentation();
        });

    builder.Logging.AddOpenTelemetry(options =>
    {
        options.SetResourceBuilder(
            ResourceBuilder.CreateDefault()
                .AddService(builder.Environment.ApplicationName));
    });
}

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IBlobStorageService, BlobStorageService>();
builder.Services.AddSingleton<ICosmosDbService>(sp =>
{
    IConfiguration configuration = sp.GetRequiredService<IConfiguration>();
    var endpointUri = configuration["CosmosDb:EndpointUri"] ?? 
        throw new InvalidOperationException("CosmosDB EndpointUri not found in configuration");
    var primaryKey = configuration["CosmosDb:PrimaryKey"] ?? 
        throw new InvalidOperationException("CosmosDB PrimaryKey not found in configuration");
    
    var cosmosClient = new CosmosClient(endpointUri, primaryKey);
    
    // Todo database config
    var todoDatabaseName = configuration["CosmosDb:Databases:Todo:Name"] ?? 
        throw new InvalidOperationException("Todo DatabaseName not found in configuration");
    var todoContainerName = configuration["CosmosDb:Databases:Todo:ContainerName"] ?? 
        throw new InvalidOperationException("Todo ContainerName not found in configuration");

    // Scan database config
    var scanDatabaseName = configuration["CosmosDb:Databases:Scan:Name"] ?? 
        throw new InvalidOperationException("Scan DatabaseName not found in configuration");
    var scanContainerName = configuration["CosmosDb:Databases:Scan:ContainerName"] ?? 
        throw new InvalidOperationException("Scan ContainerName not found in configuration");
        
    return new CosmosDbService(
        cosmosClient, 
        todoDatabaseName, 
        todoContainerName,
        scanDatabaseName,
        scanContainerName,
        sp.GetRequiredService<ILogger<CosmosDbService>>()
    );
});

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();