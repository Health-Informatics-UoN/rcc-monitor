using Azure;
using Azure.Storage.Blobs;

namespace Monitor.Services;

public class AzureStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly IHttpContextAccessor _accessor;
    private readonly LinkGenerator _generator;
    private const string Container = "synthetic-data";

    public AzureStorageService(BlobServiceClient blobServiceClient, IHttpContextAccessor accessor,
        LinkGenerator generator)
    {
        _blobServiceClient = blobServiceClient;
        _accessor = accessor;
        _generator = generator;
    }
    
    /// <summary>
    /// Get a file with a given name.
    /// </summary>
    /// <param name="name">Name of the file</param>
    /// <returns>The file as byte array.</returns>
    public async Task<byte[]> Get(string name)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(Container);
        var blobClient = containerClient.GetBlobClient(name);
        var downloadContent = await blobClient.DownloadAsync();

        using var stream = new MemoryStream();
        await downloadContent.Value.Content.CopyToAsync(stream);
        return stream.ToArray();
    }
    
    /// <summary>
    /// Upload a file to Azure blob storage.
    /// </summary>
    /// <param name="filePath">Filepath</param>
    /// <param name="dataStream">Data stream to upload</param>
    /// <returns>Uri to the uploaded file.</returns>
    public async Task<string> Upload(string filePath, Stream dataStream)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(Container);
        var blobClient = containerClient.GetBlobClient(filePath);
        await blobClient.UploadAsync(dataStream);
        return blobClient.Name;
    }

    /// <summary>
    /// Uploads a spreadsheet to Azure blob storage.
    /// </summary>
    /// <param name="data">Data to upload.</param>
    /// <param name="requestScheme"></param>
    /// <param name="requestHost"></param>
    /// <returns>The url of the file.</returns>
    /// <exception cref="RequestFailedException">Data failed to upload</exception>
    public async Task<string?> UploadSpreadsheet(byte[] data, string requestScheme, string requestHost)
    {
        try
        {
            using var stream = new MemoryStream(data);
            var filePath = $"{Guid.NewGuid()}.csv";
            var blobName = await Upload(filePath, stream);
            return _generator.GetUriByAction(_accessor.HttpContext!, "Get", "SyntheticData", new { name = blobName });
        }
        catch (Exception ex)
        {
            throw new RequestFailedException("Error uploading to Azure Storage.");
        }
    }
}