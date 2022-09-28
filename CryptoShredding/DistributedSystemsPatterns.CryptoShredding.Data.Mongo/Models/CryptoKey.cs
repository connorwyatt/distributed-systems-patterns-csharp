using MongoDB.Bson.Serialization.Attributes;

namespace DistributedSystemsPatterns.CryptoShredding.Data.Mongo.Models;

public record CryptoKey(
  [property: BsonId]
  string CryptoKeyId,
  string Key);
