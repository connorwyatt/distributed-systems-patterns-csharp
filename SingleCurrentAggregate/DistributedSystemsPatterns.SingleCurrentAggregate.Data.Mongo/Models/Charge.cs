using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using NodaTime;

namespace DistributedSystemsPatterns.SingleCurrentAggregate.Data.Mongo.Models;

public record Charge(
  [property: BsonId]
  string ChargeId,
  string BillingPeriodId,
  string UserId,
  [property: BsonRepresentation(BsonType.String)]
  ChargeStatus Status,
  double Amount,
  Instant AddedAt,
  Instant UpdatedAt,
  ulong Version);
