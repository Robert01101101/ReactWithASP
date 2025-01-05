using ReactWithASP.Server.Models;

namespace ReactWithASP.Server.Services
{
    public interface ICosmosDbService
    {
        Task<IEnumerable<ToDoItem>> GetTodoItemsAsync();
        Task AddTodoItemAsync(ToDoItem item);
        Task DeleteTodoItemAsync(string id);
        Task<IEnumerable<ScanItem>> GetScanItemsAsync();
        Task AddScanItemAsync(ScanItem item);
        Task DeleteScanItemAsync(string id);
    }
} 