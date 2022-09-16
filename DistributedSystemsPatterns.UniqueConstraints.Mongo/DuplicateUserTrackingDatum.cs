using MongoDB.Bson.Serialization.Attributes;

namespace DistributedSystemsPatterns.Mongo;

public record DuplicateUserTrackingDatum(
  [property: BsonId]
  string UserId,
  string EmailAddress);
