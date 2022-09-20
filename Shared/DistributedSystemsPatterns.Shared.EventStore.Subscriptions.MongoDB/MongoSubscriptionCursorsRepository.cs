using MongoDB.Driver;
using NodaTime;

namespace DistributedSystemsPatterns.Shared.EventStore.Subscriptions.MongoDB;

public class MongoSubscriptionCursorsRepository : ISubscriptionCursorsRepository
{
  private readonly IMongoCollection<SubscriptionCursor> _collection;
  private readonly IClock _clock;

  public MongoSubscriptionCursorsRepository(IMongoDatabase mongoDatabase, IClock clock, string collectionName)
  {
    _collection = mongoDatabase.GetCollection<SubscriptionCursor>(collectionName);
    _clock = clock;
  }

  public async Task<ulong?> GetSubscriptionCursor(string subscriberName, string streamName) =>
    (await _collection.Find(sc => sc.SubscriberName == subscriberName && sc.StreamName == streamName)
      .SingleOrDefaultAsync())?.Position;

  public async Task UpsertSubscriptionCursor(string subscriberName, string streamName, ulong position) =>
    await _collection.FindOneAndReplaceAsync<SubscriptionCursor>(
      sc => sc.SubscriberName == subscriberName && sc.StreamName == streamName,
      new SubscriptionCursor(
        GetId(subscriberName, streamName),
        subscriberName,
        streamName,
        position,
        _clock.GetCurrentInstant()),
      new FindOneAndReplaceOptions<SubscriptionCursor>
      {
        IsUpsert = true,
      });

  private static string GetId(string subscriberName, string streamName) => $"{subscriberName}:{streamName}";
}
