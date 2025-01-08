using Azure.Storage.Blobs;
using System.IO.Compression;

public interface IBlobStorageService
{
    Task<string> UploadModelFilesAsync(IFormFileCollection files);
    Task DeleteModelFilesAsync(string blobName);
    BlobContainerClient ContainerClient { get; }
}

public class BlobStorageService : IBlobStorageService
{
    private readonly BlobContainerClient _containerClient;
    public BlobContainerClient ContainerClient => _containerClient;
    private readonly ILogger<BlobStorageService> _logger;

    public BlobStorageService(IConfiguration configuration, ILogger<BlobStorageService> logger)
    {
        var connectionString = configuration["BlobStorage:ConnectionString"] 
            ?? throw new InvalidOperationException("Blob storage connection string not found");
        var containerName = configuration["BlobStorage:ContainerName"] 
            ?? throw new InvalidOperationException("Blob storage container name not found");
        
        _containerClient = new BlobContainerClient(connectionString, containerName);
        _logger = logger;
    }

    public async Task<string> UploadModelFilesAsync(IFormFileCollection files)
    {
        // Create a unique folder name for this upload
        string folderName = Guid.NewGuid().ToString();
        
        // Create a memory stream for the zip file
        using var zipStream = new MemoryStream();
        using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
        {
            foreach (var file in files)
            {
                var entry = archive.CreateEntry(file.FileName);
                using var entryStream = entry.Open();
                await file.CopyToAsync(entryStream);
            }
        }

        // Reset stream position
        zipStream.Position = 0;

        // Upload the zip file to blob storage
        string blobName = $"{folderName}/model.zip";
        var blobClient = _containerClient.GetBlobClient(blobName);
        await blobClient.UploadAsync(zipStream, true);

        return blobClient.Uri.ToString();
    }

    public async Task DeleteModelFilesAsync(string blobUrl)
    {
        if (Uri.TryCreate(blobUrl, UriKind.Absolute, out Uri? uri))
        {
            string blobName = uri.Segments.Last();
            var blobClient = _containerClient.GetBlobClient(blobName);
            await blobClient.DeleteIfExistsAsync();
        }
    }
}