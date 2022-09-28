using DistributedSystemsPatterns.CryptoShredding.Data.Mongo.Mapping;
using DistributedSystemsPatterns.CryptoShredding.Data.Mongo.Models;
using MongoDB.Driver;

namespace DistributedSystemsPatterns.CryptoShredding.Data.Mongo;

public class MongoCryptoKeysRepository : ICryptoKeysRepository
{
  private const string CollectionName = "crypto_keys";

  private readonly IMongoCollection<CryptoKey> _collection;

  public MongoCryptoKeysRepository(IMongoDatabase database) =>
    _collection = database.GetCollection<CryptoKey>(CollectionName);

  public async Task<Data.Models.CryptoKey?> GetCryptoKey(string cryptoKeyId)
  {
    var cryptoKey = await _collection.Find(c => c.CryptoKeyId == cryptoKeyId).SingleOrDefaultAsync();
    return cryptoKey is not null ? CryptoKeyMapper.ToDataModel(cryptoKey) : null;
  }

  public async Task<IList<Data.Models.CryptoKey>> GetCryptoKeys()
  {
    var cryptoKeysQuery = _collection.Find(FilterDefinition<CryptoKey>.Empty);
    var cryptoKeys = await cryptoKeysQuery.ToListAsync();
    return cryptoKeys.Select(CryptoKeyMapper.ToDataModel).ToArray();
  }

  public async Task InsertCryptoKey(Data.Models.CryptoKey cryptoKey)
  {
    await _collection.InsertOneAsync(CryptoKeyMapper.FromDataModel(cryptoKey));
  }

  public async Task DeleteCryptoKey(string cryptoKeyId)
  {
    await _collection.DeleteOneAsync(cryptoKey => cryptoKey.CryptoKeyId == cryptoKeyId);
  }
}
