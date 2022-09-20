using System.Text.Json.Serialization;
using NodaTime;

namespace DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.Controllers.Models;

public record BillingPeriod(
  string BillingPeriodId,
  string UserId,
  [property: JsonConverter(typeof(JsonStringEnumConverter))]
  BillingPeriodStatus Status,
  double TotalAmount,
  Instant OpenedAt,
  Instant? ClosedAt,
  Instant UpdatedAt);
