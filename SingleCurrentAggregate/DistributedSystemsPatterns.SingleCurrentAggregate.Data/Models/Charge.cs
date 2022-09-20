using NodaTime;

namespace DistributedSystemsPatterns.SingleCurrentAggregate.Data.Models;

public record Charge(
  string ChargeId,
  string BillingPeriodId,
  string UserId,
  ChargeStatus Status,
  double Amount,
  Instant AddedAt,
  Instant UpdatedAt,
  ulong Version);
