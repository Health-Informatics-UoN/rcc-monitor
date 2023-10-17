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
    
    /// <summary>
    /// Upload a file to storage.
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="dataStream"></param>
    /// <returns></returns>
    public async Task<string> Upload(string filePath, Stream dataStream)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(Container);
        var blobClient = containerClient.GetBlobClient(filePath);
        await blobClient.UploadAsync(dataStream);
        return blobClient.Name;
    }
}