using System.Text.Json.Serialization;
using NodaTime;

namespace DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.Controllers.Models;

public record Charge(
  string ChargeId,
  string BillingPeriodId,
  string UserId,
  [property: JsonConverter(typeof(JsonStringEnumConverter))]
  ChargeStatus Status,
  double Amount,
  Instant AddedAt,
  Instant UpdatedAt);
