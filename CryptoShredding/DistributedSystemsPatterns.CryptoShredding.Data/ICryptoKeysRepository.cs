using DistributedSystemsPatterns.CryptoShredding.Data.Models;

namespace DistributedSystemsPatterns.CryptoShredding.Data;

public interface ICryptoKeysRepository
{
  Task<CryptoKey?> GetCryptoKey(string cryptoKeyId);

  Task<IList<CryptoKey>> GetCryptoKeys();

  Task InsertCryptoKey(CryptoKey cryptoKey);

  Task DeleteCryptoKey(string cryptoKeyId);
}
