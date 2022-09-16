using MongoDB.Driver;

namespace DistributedSystemsPatterns.Mongo;

public class MongoDuplicateUserTrackingDataRepository
{
  private const string CollectionName = "duplicate_user_tracking_data";

  private readonly IMongoCollection<DuplicateUserTrackingDatum> _collection;

  public MongoDuplicateUserTrackingDataRepository(IMongoDatabase database) =>
    _collection = database.GetCollection<DuplicateUserTrackingDatum>(CollectionName);

  public async Task<bool> HasUserWithEmailAddress(string emailAddress)
  {
    return await _collection.CountDocumentsAsync(d => d.EmailAddress == emailAddress) > 0;
  }

  public async Task AddUser(DuplicateUserTrackingDatum duplicateUserTrackingDatum)
  {
    await _collection.InsertOneAsync(duplicateUserTrackingDatum);
  }
}
