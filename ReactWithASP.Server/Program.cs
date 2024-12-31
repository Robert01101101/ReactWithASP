using Microsoft.Azure.Cosmos;
using ReactWithASP.Server.Services;
using Microsoft.Extensions.Logging;

//Config: To run locally, make sure Multiple startup projects is selected under solution properties, with action Start for both

var builder = WebApplication.CreateBuilder(args);

// Add logging configuration
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .AddConsole()
        .AddDebug()
        .SetMinimumLevel(LogLevel.Information);
});

var logger = loggerFactory.CreateLogger<Program>();

// Add environment logging
logger.LogInformation("=== Environment Information ===");
logger.LogInformation("Current environment: {Environment}", builder.Environment.EnvironmentName);
logger.LogInformation("Is Development: {IsDevelopment}", builder.Environment.IsDevelopment());
logger.LogInformation("Content Root Path: {ContentRootPath}", builder.Environment.ContentRootPath);

// Explicitly set environment if needed
if (builder.Environment.IsDevelopment())
{
    logger.LogInformation("=== Loading Development Configuration ===");
    builder.Configuration
        .SetBasePath(builder.Environment.ContentRootPath)
        .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables();
    
    // Log the configuration source
    logger.LogInformation("Configuration files:");
    foreach (var source in builder.Configuration.Sources)
    {
        logger.LogInformation("  - {Source}", source);
    }
}

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton(sp =>
{
    var configuration = sp.GetService<IConfiguration>() ?? 
        throw new InvalidOperationException("Configuration not found");
        
    var endpointUri = configuration["CosmosDb:EndpointUri"] ?? 
        throw new InvalidOperationException("CosmosDB EndpointUri not found in configuration");
    var primaryKey = configuration["CosmosDb:PrimaryKey"] ?? 
        throw new InvalidOperationException("CosmosDB PrimaryKey not found in configuration");
    
    var cosmosClient = new CosmosClient(endpointUri, primaryKey);
    
    var databaseName = configuration["CosmosDb:DatabaseName"] ?? 
        throw new InvalidOperationException("CosmosDB DatabaseName not found in configuration");
    var containerName = configuration["CosmosDb:ContainerName"] ?? 
        throw new InvalidOperationException("CosmosDB ContainerName not found in configuration");
        
    return new CosmosDbService(cosmosClient, databaseName, containerName, 
        sp.GetRequiredService<ILogger<CosmosDbService>>());
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