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
        private readonly CosmosDbService _cosmosDbService;
        private readonly ILogger<ToDoController> _logger;

        public ToDoController(CosmosDbService cosmosDbService, ILogger<ToDoController> logger)
        {
            _cosmosDbService = cosmosDbService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            IEnumerable<ToDoItem> items = await _cosmosDbService.GetItemsAsync();
            return Ok(items);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ToDoItem item)
        {
            _logger.LogInformation("Received POST request with item: {@Item}", item);
            
            try 
            {
                await _cosmosDbService.AddItemAsync(item);
                _logger.LogInformation("Successfully added item with ID: {Id}", item.Id);
                return CreatedAtAction(nameof(Get), new { id = item.Id }, item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding todo item");
                return StatusCode(500, "Error adding todo item");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try 
            {
                await _cosmosDbService.DeleteItemAsync(id);
                _logger.LogInformation("Successfully deleted item with ID: {Id}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting todo item");
                return StatusCode(500, "Error deleting todo item");
            }
        }
    }
} 