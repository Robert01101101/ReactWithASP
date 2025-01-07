using Microsoft.AspNetCore.Mvc;
using ReactWithASP.Server.Models;
using ReactWithASP.Server.Services;
using Microsoft.Extensions.Logging;

namespace ReactWithASP.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScanController : ControllerBase
    {
        private readonly ICosmosDbService _cosmosDbService;
        private readonly ILogger<ScanController> _logger;

        public ScanController(ICosmosDbService cosmosDbService, ILogger<ScanController> logger)
        {
            _cosmosDbService = cosmosDbService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            IEnumerable<ScanItem> items = await _cosmosDbService.GetScanItemsAsync();
            return Ok(items);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ScanItem item)
        {
            _logger.LogInformation("Received POST request with scan item: {@Item}", item);
            try 
            {
                await _cosmosDbService.AddScanItemAsync(item);
                _logger.LogInformation("Successfully added scan item with ID: {Id}", item.Id);
                return CreatedAtAction(nameof(Get), new { id = item.Id }, item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding scan item");
                return StatusCode(500, "Error adding scan item");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try 
            {
                await _cosmosDbService.DeleteScanItemAsync(id);
                _logger.LogInformation("Successfully deleted scan item with ID: {Id}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting scan item");
                return StatusCode(500, "Error deleting scan item");
            }
        }
    }
} 