using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.EntityFrameworkCore.DataEncryption.Providers;

namespace Monitor.Data.Crypto;

/// <summary>
/// We need a custom AesProvider that supports dynamic IV, as the base library does not do it.
/// Why this is necessary: https://stackoverflow.com/questions/1220751/how-to-choose-an-aes-encryption-mode-cbc-ecb-ctr-ocb-cfb/42658861#42658861
/// Source: https://gist.github.com/DerStimmler/ee6ae4b10c9d2e58dc5c63e087e1fb62
/// </summary>
public class DynamicIvAesProvider : IEncryptionProvider
{
  /// <summary>
  ///   AES block size constant.
  /// </summary>
  private const int AesBlockSize = 128;

  /// <summary>
  ///   Initialization vector size constant.
  /// </summary>
  private const int InitializationVectorSize = 16;

  private readonly byte[] _key;
  private readonly CipherMode _mode;
  private readonly PaddingMode _padding;

  /// <summary>
  ///   Creates a new <see cref="AesProvider" /> instance used to perform symmetric encryption and decryption on strings.
  /// </summary>
  /// <param name="key">AES key used for the symmetric encryption.</param>
  /// <param name="mode">Mode for operation used in the symmetric encryption.</param>
  /// <param name="padding">Padding mode used in the symmetric encryption.</param>
  public DynamicIvAesProvider(byte[] key, CipherMode mode = CipherMode.CBC, PaddingMode padding = PaddingMode.ISO10126)
  {
    _key = key;
    _mode = mode;
    _padding = padding;
  }

  public byte[]? Encrypt(byte[]? input)
  {
    if (input is null || input.Length == 0) return default;

    using var aes = CreateAes();
    using var memoryStream = new MemoryStream();

    aes.GenerateIV();
    var initializationVector = aes.IV;
    memoryStream.Write(initializationVector, 0, initializationVector.Length);

    using var transform = aes.CreateEncryptor(_key, initializationVector);
    using var crypto = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
    crypto.Write(input, 0, input.Length);
    crypto.FlushFinalBlock();

    memoryStream.Seek(0L, SeekOrigin.Begin);
    return StreamToBytes(memoryStream);
  }

  public byte[]? Decrypt(byte[]? input)
  {
    if (input is null || input.Length == 0) return default;

    using var memoryStream = new MemoryStream(input);

    var initializationVector = new byte[InitializationVectorSize];
    var unused = memoryStream.Read(initializationVector, 0, initializationVector.Length);

    using var aes = CreateAes();
    using var transform = aes.CreateDecryptor(_key, initializationVector);
    using var crypto = new CryptoStream(memoryStream, transform, CryptoStreamMode.Read);

    return StreamToBytes(crypto);
  }

  /// <summary>
  ///   Generates an AES cryptography provider.
  /// </summary>
  /// <returns></returns>
  private Aes CreateAes()
  {
    var aes = Aes.Create();
    aes.BlockSize = AesBlockSize;
    aes.Mode = _mode;
    aes.Padding = _padding;
    aes.Key = _key;
    aes.KeySize = _key.Length * 8;

    return aes;
  }

  private static byte[] StreamToBytes(Stream stream)
  {
    if (stream is MemoryStream ms)
      return ms.ToArray();

    using var output = new MemoryStream();
    stream.CopyTo(output);

    return output.ToArray();
  }
}