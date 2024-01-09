namespace Monitor.Data.Config;

public class EncryptionOptions
{
    /// <summary>
    /// A 16 (AES-128) or 32 (AES-256) byte key.
    /// </summary>
    public string EncryptionKey { get; init; } = string.Empty;

}