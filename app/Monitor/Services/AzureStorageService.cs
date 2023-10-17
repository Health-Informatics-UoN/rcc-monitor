using Azure.Storage.Blobs;

namespace Monitor.Services;

public class AzureStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private const string Container = "synthetic-data";

    public AzureStorageService(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }
    
    public async Task<byte[]> Get(string filePath)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(Container);
        var blobClient = containerClient.GetBlobClient(filePath);
        var downloadContent = await blobClient.DownloadAsync();

        using var stream = new MemoryStream();
        await downloadContent.Value.Content.CopyToAsync(stream);
        return stream.ToArray();
    }
    
    /// <summary>
    /// Upload a file to storage.
    /// </summary>
    /// <param name="filePath">Filepath</param>
    /// <param name="dataStream">Data stream to upload</param>
    /// <returns>Uri to the uploaded file.</returns>
    public async Task<Uri> Upload(string filePath, Stream dataStream)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(Container);
        var blobClient = containerClient.GetBlobClient(filePath);
        await blobClient.UploadAsync(dataStream);
        return blobClient.Uri;
    }
}