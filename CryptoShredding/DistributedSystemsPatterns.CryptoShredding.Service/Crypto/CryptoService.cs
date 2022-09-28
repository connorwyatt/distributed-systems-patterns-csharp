using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using DistributedSystemsPatterns.CryptoShredding.Data;
using DistributedSystemsPatterns.CryptoShredding.Data.Models;

namespace DistributedSystemsPatterns.CryptoShredding.Service.Crypto;

public class CryptoService
{
  private static readonly byte[] IV = { 53, 213, 50, 215, 13, 98, 130, 229, 126, 2, 105, 33, 201, 111, 157, 136, };

  private const string DefaultValue = "<REDACTED>";

  private static readonly Regex EncryptedDataRegex = new("^EncryptedData<(.+)>$");

  private readonly ICryptoKeysRepository _cryptoKeysRepository;

  public CryptoService(ICryptoKeysRepository cryptoKeysRepository) => _cryptoKeysRepository = cryptoKeysRepository;

  public async Task<string> Encrypt(string cryptoKeyId, string value)
  {
    var valueBytes = Encoding.UTF8.GetBytes(value);

    using var aes = CreateAes();

    await StoreCryptoKey(cryptoKeyId, aes.Key);

    using var cryptoTransform = aes.CreateEncryptor();

    var encryptedValueBytes = cryptoTransform.TransformFinalBlock(valueBytes, 0, valueBytes.Length);

    var encryptedValue = Convert.ToBase64String(encryptedValueBytes);

    return CreateEncryptedDataString(encryptedValue);
  }

  public async Task<string> Decrypt(string cryptoKeyId, string encryptedData)
  {
    var encryptedValue = ParseEncryptedDataString(encryptedData);

    var cryptoKey = await _cryptoKeysRepository.GetCryptoKey(cryptoKeyId);

    if (cryptoKey is null)
    {
      return DefaultValue;
    }

    var encryptedValueBytes = Convert.FromBase64String(encryptedValue);

    using var aes = CreateAes(Convert.FromBase64String(cryptoKey.Key));

    using var cryptoTransform = aes.CreateDecryptor();

    var valueBytes = cryptoTransform.TransformFinalBlock(encryptedValueBytes, 0, encryptedValueBytes.Length);

    return Encoding.UTF8.GetString(valueBytes);
  }

  public async Task Redact(string cryptoKeyId)
  {
    await _cryptoKeysRepository.DeleteCryptoKey(cryptoKeyId);
  }

  private static Aes CreateAes(byte[]? key = null)
  {
    var aes = Aes.Create();

    aes.Mode = CipherMode.CBC;
    aes.IV = IV;

    if (key is not null)
    {
      aes.Key = key;
    }
    else
    {
      aes.GenerateKey();
    }

    return aes;
  }

  private async Task StoreCryptoKey(string cryptoKeyId, byte[] key)
  {
    var keyString = Convert.ToBase64String(key);

    var cryptoKey = new CryptoKey(cryptoKeyId, keyString);

    await _cryptoKeysRepository.InsertCryptoKey(cryptoKey);
  }

  private static string CreateEncryptedDataString(string encryptedValue) => $"EncryptedData<{encryptedValue}>";

  private static string ParseEncryptedDataString(string encryptedValue)
  {
    var match = EncryptedDataRegex.Match(encryptedValue);

    if (!match.Success)
    {
      throw new InvalidOperationException("Could not parse encrypted value.");
    }

    return match.Groups[1].Value;
  }
}
