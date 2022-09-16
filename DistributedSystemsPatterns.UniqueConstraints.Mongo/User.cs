using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using NodaTime;

namespace DistributedSystemsPatterns.Mongo;

public record User(
  [property: BsonId]
  string UserId,
  [property: BsonRepresentation(BsonType.String)]
  UserStatus Status,
  string Name,
  string EmailAddress,
  Instant JoinedAt,
  ulong Version);
