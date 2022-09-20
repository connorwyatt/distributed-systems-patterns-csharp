using MongoDB.Bson.Serialization.Attributes;
using NodaTime;

namespace DistributedSystemsPatterns.Shared.EventStore.Subscriptions.MongoDB;

public record SubscriptionCursor(
  [property: BsonId]
  string Id,
  string SubscriberName,
  string StreamName,
  ulong Position,
  Instant LastUpdated);
