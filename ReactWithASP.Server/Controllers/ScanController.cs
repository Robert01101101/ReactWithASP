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
        private readonly IBlobStorageService _blobStorageService;
        private readonly ILogger<ScanController> _logger;

        public ScanController(ICosmosDbService cosmosDbService, IBlobStorageService blobStorageService, ILogger<ScanController> logger)
        {
            _cosmosDbService = cosmosDbService;
            _blobStorageService = blobStorageService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            IEnumerable<ScanItem> items = await _cosmosDbService.GetScanItemsAsync();
            return Ok(items);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromForm] IFormCollection form)
        {
            try
            {
                // Validate required files
                if (!form.Files.Any(f => Path.GetExtension(f.FileName).ToLower() == ".obj"))
                {
                    return BadRequest("An OBJ file is required");
                }

                // Upload files to blob storage
                string blobUrl = await _blobStorageService.UploadModelFilesAsync(form.Files);

                // Create scan item
                var scanItem = new ScanItem
                {
                    Title = form["title"],
                    Subject = form["subject"],
                    Description = form["description"],
                    BlobUrl = blobUrl,
                    OriginalFileName = form.Files.First(f => Path.GetExtension(f.FileName).ToLower() == ".obj").FileName
                };

                await _cosmosDbService.AddScanItemAsync(scanItem);
                return CreatedAtAction(nameof(Get), new { id = scanItem.Id }, scanItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing scan upload");
                return StatusCode(500, "Error processing scan upload");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                // Get the scan item to get its blob URL
                var scan = await _cosmosDbService.GetScanItemAsync(id);
                if (scan?.BlobUrl != null)
                {
                    await _blobStorageService.DeleteModelFilesAsync(scan.BlobUrl);
                }
                await _cosmosDbService.DeleteScanItemAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting scan");
                return StatusCode(500, "Error deleting scan");
            }
        }
    }
} 