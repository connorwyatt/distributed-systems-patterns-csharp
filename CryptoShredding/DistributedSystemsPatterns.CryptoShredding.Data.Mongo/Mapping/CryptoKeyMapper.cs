using DistributedSystemsPatterns.CryptoShredding.Data.Models;

namespace DistributedSystemsPatterns.CryptoShredding.Data.Mongo.Mapping;

public static class CryptoKeyMapper
{
  public static CryptoKey ToDataModel(Models.CryptoKey cryptoKey) => new(cryptoKey.CryptoKeyId, cryptoKey.Key);

  public static Models.CryptoKey FromDataModel(CryptoKey cryptoKey) => new(cryptoKey.CryptoKeyId, cryptoKey.Key);
}
