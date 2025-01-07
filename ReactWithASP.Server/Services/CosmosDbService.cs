using Microsoft.Azure.Cosmos;
using ReactWithASP.Server.Models;
using System.Collections.Concurrent;
using System.ComponentModel;
using Microsoft.Extensions.Logging;

namespace ReactWithASP.Server.Services
{
    public class CosmosDbService : ICosmosDbService
    {
        private readonly Microsoft.Azure.Cosmos.Container _todoContainer;
        private readonly Microsoft.Azure.Cosmos.Container _scanContainer;
        private readonly ILogger<CosmosDbService> _logger;

        public CosmosDbService(
            CosmosClient cosmosClient,
            string todoDatabaseName,
            string todoContainerName,
            string scanDatabaseName,
            string scanContainerName,
            ILogger<CosmosDbService> logger)
        {
            _todoContainer = cosmosClient.GetContainer(todoDatabaseName, todoContainerName);
            _scanContainer = cosmosClient.GetContainer(scanDatabaseName, scanContainerName);
            _logger = logger;
        }

        // Todo Methods
        public async Task<IEnumerable<ToDoItem>> GetTodoItemsAsync()
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM c");
            var query = _todoContainer.GetItemQueryIterator<ToDoItem>(queryDefinition);
            var results = new List<ToDoItem>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }
            return results;
        }

        public async Task AddTodoItemAsync(ToDoItem item)
        {
            try
            {
                item.Id = item.Id?.ToLowerInvariant() ?? Guid.NewGuid().ToString().ToLowerInvariant();
                item.CreatedAt = DateTime.UtcNow;

                _logger.LogInformation("Creating todo item with ID: {Id}", item.Id);

                var partitionKey = new PartitionKey(item.Id);
                await _todoContainer.CreateItemAsync(item, partitionKey);
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex, "Cosmos DB Error: {Message}, StatusCode: {StatusCode}",
                    ex.Message, ex.StatusCode);
                throw;
            }
        }

        public async Task DeleteTodoItemAsync(string id)
        {
            try
            {
                await _todoContainer.DeleteItemAsync<ToDoItem>(id, new PartitionKey(id));
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex, "Cosmos DB Error: {Message}, StatusCode: {StatusCode}",
                    ex.Message, ex.StatusCode);
                throw;
            }
        }

        // Scan Methods
        public async Task<IEnumerable<ScanItem>> GetScanItemsAsync()
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM c");
            var query = _scanContainer.GetItemQueryIterator<ScanItem>(queryDefinition);
            var results = new List<ScanItem>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }
            return results;
        }

        public async Task AddScanItemAsync(ScanItem item)
        {
            try
            {
                item.Id = Guid.NewGuid().ToString().ToLowerInvariant();
                item.CreatedAt = DateTime.UtcNow;

                _logger.LogInformation("Creating scan item with ID: {Id}", item.Id);

                var partitionKey = new PartitionKey(item.Id);
                await _scanContainer.CreateItemAsync(item, partitionKey);
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex, "Cosmos DB Error: {Message}, StatusCode: {StatusCode}",
                    ex.Message, ex.StatusCode);
                throw;
            }
        }

        public async Task DeleteScanItemAsync(string id)
        {
            try
            {
                await _scanContainer.DeleteItemAsync<ScanItem>(id, new PartitionKey(id));
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex, "Cosmos DB Error: {Message}, StatusCode: {StatusCode}",
                    ex.Message, ex.StatusCode);
                throw;
            }
        }

        public async Task<ScanItem?> GetScanItemAsync(string id)
        {
            try
            {
                var response = await _scanContainer.ReadItemAsync<ScanItem>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex, "Cosmos DB Error: {Message}, StatusCode: {StatusCode}",
                    ex.Message, ex.StatusCode);
                throw;
            }
        }
    }
}