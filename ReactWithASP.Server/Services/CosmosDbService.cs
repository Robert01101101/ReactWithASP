using Microsoft.Azure.Cosmos;
using ReactWithASP.Server.Models;
using System.Collections.Concurrent;
using System.ComponentModel;
using Microsoft.Extensions.Logging;

namespace ReactWithASP.Server.Services
{
    public class CosmosDbService
    {
        private readonly Microsoft.Azure.Cosmos.Container _container;
        private readonly ILogger<CosmosDbService> _logger;

        public CosmosDbService(
            CosmosClient cosmosClient,
            string databaseName,
            string containerName,
            ILogger<CosmosDbService> logger)
        {
            _container = cosmosClient.GetContainer(databaseName, containerName);
            _logger = logger;
        }

        public async Task<IEnumerable<ToDoItem>> GetItemsAsync()
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM c");
            var query = _container.GetItemQueryIterator<ToDoItem>(queryDefinition);
            var results = new List<ToDoItem>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }
            return results;
        }

        public async Task AddItemAsync(ToDoItem item)
        {
            try 
            {
                // Ensure ID is lowercase and exists
                item.Id = item.Id?.ToLowerInvariant() ?? Guid.NewGuid().ToString().ToLowerInvariant();
                item.CreatedAt = DateTime.UtcNow;
                
                _logger.LogInformation("Creating item with ID: {Id}", item.Id);
                
                // Use ID as partition key
                var partitionKey = new PartitionKey(item.Id);
                var itemRequestOptions = new ItemRequestOptions { EnableContentResponseOnWrite = true };
                
                await _container.CreateItemAsync(item, partitionKey, itemRequestOptions);
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex, "Cosmos DB Error: {Message}, StatusCode: {StatusCode}", 
                    ex.Message, ex.StatusCode);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "General Error: {Message}", ex.Message);
                throw;
            }
        }

        public async Task DeleteItemAsync(string id)
        {
            try 
            {
                await _container.DeleteItemAsync<ToDoItem>(id, new PartitionKey(id));
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex, "Cosmos DB Error: {Message}, StatusCode: {StatusCode}", 
                    ex.Message, ex.StatusCode);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "General Error: {Message}", ex.Message);
                throw;
            }
        }
    }
}