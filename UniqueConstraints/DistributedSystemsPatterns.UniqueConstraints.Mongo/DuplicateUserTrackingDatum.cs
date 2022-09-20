using MongoDB.Bson.Serialization.Attributes;

namespace DistributedSystemsPatterns.UniqueConstraints.Mongo;

public record DuplicateUserTrackingDatum(
  [property: BsonId]
  string UserId,
  string EmailAddress);
