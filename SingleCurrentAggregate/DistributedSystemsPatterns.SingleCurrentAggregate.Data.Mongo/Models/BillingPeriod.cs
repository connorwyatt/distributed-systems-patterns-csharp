using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using NodaTime;

namespace DistributedSystemsPatterns.SingleCurrentAggregate.Data.Mongo.Models;

public record BillingPeriod(
  [property: BsonId]
  string BillingPeriodId,
  string UserId,
  [property: BsonRepresentation(BsonType.String)]
  BillingPeriodStatus Status,
  double TotalAmount,
  Instant OpenedAt,
  Instant? ClosedAt,
  Instant UpdatedAt,
  ulong Version);
