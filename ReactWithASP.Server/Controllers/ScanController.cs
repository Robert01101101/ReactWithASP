using Microsoft.AspNetCore.Mvc;
using ReactWithASP.Server.Models;
using ReactWithASP.Server.Services;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;

namespace ReactWithASP.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScanController : ControllerBase
    {
        private readonly ICosmosDbService _cosmosDbService;
        private readonly IBlobStorageService _blobStorageService;
        private readonly BlobContainerClient _containerClient;
        private readonly ILogger<ScanController> _logger;

        public ScanController(
            ICosmosDbService cosmosDbService, 
            IBlobStorageService blobStorageService, 
            ILogger<ScanController> logger)
        {
            _cosmosDbService = cosmosDbService;
            _blobStorageService = blobStorageService;
            _containerClient = blobStorageService.ContainerClient;
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
                _logger.LogInformation("Starting scan upload with title: {Title}", form["title"]);

                // Validate required files
                if (!form.Files.Any(f => Path.GetExtension(f.FileName).ToLower() == ".obj"))
                {
                    _logger.LogWarning("Upload rejected: No OBJ file provided");
                    return BadRequest("An OBJ file is required");
                }

                _logger.LogInformation("Uploading {Count} files for scan", form.Files.Count);
                // Upload files to blob storage
                string blobUrl = await _blobStorageService.UploadModelFilesAsync(form.Files);
                _logger.LogInformation("Files uploaded successfully to blob storage: {BlobUrl}", blobUrl);

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
                _logger.LogInformation("Successfully created scan item with ID: {Id}", scanItem.Id);

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
                _logger.LogInformation("Starting delete operation for scan with ID: {Id}", id);
                
                // Get the scan item to get its blob URL
                var scan = await _cosmosDbService.GetScanItemAsync(id);
                if (scan?.BlobUrl != null)
                {
                    _logger.LogInformation("Deleting blob for scan ID: {Id}, BlobUrl: {BlobUrl}", id, scan.BlobUrl);
                    await _blobStorageService.DeleteModelFilesAsync(scan.BlobUrl);
                }
                await _cosmosDbService.DeleteScanItemAsync(id);
                
                _logger.LogInformation("Successfully deleted scan with ID: {Id}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting scan with ID: {Id}", id);
                return StatusCode(500, "Error deleting scan");
            }
        }

        [HttpGet("{id}/download")]
        public async Task<IActionResult> Download(string id)
        {
            try
            {
                _logger.LogInformation("Starting download operation for scan with ID: {Id}", id);
                
                ScanItem? scan = await _cosmosDbService.GetScanItemAsync(id);
                if (scan == null || string.IsNullOrEmpty(scan.BlobUrl))
                {
                    _logger.LogWarning("Scan not found or has no blob URL. ID: {Id}", id);
                    return NotFound();
                }

                _logger.LogInformation("Retrieving blob for scan ID: {Id}, BlobUrl: {BlobUrl}", id, scan.BlobUrl);

                // Extract the relative path from the full URL
                var blobUri = new Uri(scan.BlobUrl);
                var containerUri = _containerClient.Uri;
                var relativePath = blobUri.AbsolutePath.Substring(containerUri.AbsolutePath.Length + 1);
                
                var blobClient = _containerClient.GetBlobClient(relativePath);
                var download = await blobClient.DownloadAsync();

                _logger.LogInformation("Successfully retrieved blob for scan ID: {Id}", id);
                return File(download.Value.Content, "application/zip", $"{scan.Title}.zip");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading scan with ID: {Id}", id);
                return StatusCode(500, "Error downloading scan");
            }
        }
    }
} 