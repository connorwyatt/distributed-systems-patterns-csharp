using NodaTime;

namespace DistributedSystemsPatterns.SingleCurrentAggregate.Data.Models;

public record BillingPeriod(
  string BillingPeriodId,
  string UserId,
  BillingPeriodStatus Status,
  double TotalAmount,
  Instant OpenedAt,
  Instant? ClosedAt,
  Instant UpdatedAt,
  ulong Version);
