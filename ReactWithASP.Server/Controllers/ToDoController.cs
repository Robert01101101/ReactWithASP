using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReactWithASP.Server.Models;
using ReactWithASP.Server.Services;
using Microsoft.Extensions.Logging;

namespace ReactWithASP.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoController : ControllerBase
    {
        private readonly ICosmosDbService _cosmosDbService;
        private readonly ILogger<ToDoController> _logger;

        public ToDoController(ICosmosDbService cosmosDbService, ILogger<ToDoController> logger)
        {
            _cosmosDbService = cosmosDbService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                _logger.LogInformation("Starting GET request for todos");
                IEnumerable<ToDoItem> items = await _cosmosDbService.GetTodoItemsAsync();
                _logger.LogInformation("Successfully retrieved {Count} todos", items?.Count() ?? 0);
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred while fetching todos");
                return StatusCode(500, new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ToDoItem item)
        {
            _logger.LogInformation("Starting todo creation with title: {Title}", item.Title);
            
            try 
            {
                await _cosmosDbService.AddTodoItemAsync(item);
                _logger.LogInformation("Successfully created todo item. ID: {Id}, Title: {Title}", item.Id, item.Title);
                return CreatedAtAction(nameof(Get), new { id = item.Id }, item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding todo item with title: {Title}", item.Title);
                return StatusCode(500, "Error adding todo item");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try 
            {
                _logger.LogInformation("Starting delete operation for todo with ID: {Id}", id);
                await _cosmosDbService.DeleteTodoItemAsync(id);
                _logger.LogInformation("Successfully deleted todo with ID: {Id}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting todo item with ID: {Id}", id);
                return StatusCode(500, "Error deleting todo item");
            }
        }
    }
} 