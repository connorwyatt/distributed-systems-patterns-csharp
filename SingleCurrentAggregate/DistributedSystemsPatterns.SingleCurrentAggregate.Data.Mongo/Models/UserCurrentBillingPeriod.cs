using MongoDB.Bson.Serialization.Attributes;

namespace DistributedSystemsPatterns.SingleCurrentAggregate.Data.Mongo.Models;

public record UserCurrentBillingPeriod(
  [property: BsonId]
  string UserId,
  string BillingPeriodId);
