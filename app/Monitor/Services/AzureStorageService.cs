using Azure.Storage.Blobs;
using Flurl;
using Microsoft.AspNetCore.Mvc;
using Monitor.Exceptions;

namespace Monitor.Services;

public class AzureStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly IUrlHelper _urlHelper;
    private const string Container = "synthetic-data";

    public AzureStorageService(BlobServiceClient blobServiceClient, IUrlHelper urlHelper)
    {
        _blobServiceClient = blobServiceClient;
        _urlHelper = urlHelper;
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
    /// Upload a file to storage.
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
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="requestScheme"></param>
    /// <param name="requestHost"></param>
    /// <returns></returns>
    /// <exception cref="DataUploadException"></exception>
    public async Task<string?> UploadSpreadsheet(byte[] data, string requestScheme, string requestHost)
    {
        try
        {
            using var stream = new MemoryStream(data);
            var filePath = $"{Guid.NewGuid()}.csv";
            var blobName = await Upload(filePath, stream);
            return _urlHelper.Action("Get", "SyntheticData", new { name = blobName }, requestScheme, requestHost);
        }
        catch (Exception ex)
        {
            throw new DataUploadException("Error uploading to Azure Storage.", ex);
        }
    }
}