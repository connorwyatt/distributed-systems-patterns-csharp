using MongoDB.Bson.Serialization.Attributes;

namespace DistributedSystemsPatterns.CryptoShredding.Data.Mongo.Models;

public record Customer(
  [property: BsonId]
  string CustomerId,
  string Name,
  string SensitivePersonalInformation,
  ulong Version);
