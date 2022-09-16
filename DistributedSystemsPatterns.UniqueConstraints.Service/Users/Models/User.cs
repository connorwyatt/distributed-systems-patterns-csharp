using System.Text.Json.Serialization;
using NodaTime;

namespace DistributedSystemsPatterns.UniqueConstraints.Service.Users.Models;

public record User(
  string UserId,
  [property: JsonConverter(typeof(JsonStringEnumConverter))]
  UserStatus Status,
  string Name,
  string EmailAddress,
  Instant JoinedAt);
