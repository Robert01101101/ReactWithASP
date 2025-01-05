using Microsoft.Azure.Cosmos;
using ReactWithASP.Server.Models;
using Microsoft.Extensions.Logging;

namespace ReactWithASP.Server.Services
{
    public class CosmosDbService : ICosmosDbService
    {
        private readonly Container _todoContainer;
        private readonly Container _scanContainer;
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
                var itemRequestOptions = new ItemRequestOptions { EnableContentResponseOnWrite = true };
                
                await _todoContainer.CreateItemAsync(item, partitionKey, itemRequestOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating todo item");
                throw;
            }
        }

        public async Task DeleteTodoItemAsync(string id)
        {
            try
            {
                await _todoContainer.DeleteItemAsync<ToDoItem>(id, new PartitionKey(id));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting todo item");
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
                item.Id = item.Id?.ToLowerInvariant() ?? Guid.NewGuid().ToString().ToLowerInvariant();
                item.CreatedAt = DateTime.UtcNow;
                
                _logger.LogInformation("Creating scan item with ID: {Id}", item.Id);
                
                var partitionKey = new PartitionKey(item.Id);
                var itemRequestOptions = new ItemRequestOptions { EnableContentResponseOnWrite = true };
                
                await _scanContainer.CreateItemAsync(item, partitionKey, itemRequestOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating scan item");
                throw;
            }
        }

        public async Task DeleteScanItemAsync(string id)
        {
            try
            {
                await _scanContainer.DeleteItemAsync<ScanItem>(id, new PartitionKey(id));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting scan item");
                throw;
            }
        }
    }
}