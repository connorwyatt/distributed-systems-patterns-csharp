using DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.Controllers.Models;

namespace DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.Controllers.Mapping;

public static class ChargeStatusMapper
{
  public static ChargeStatus ToApiModel(Data.Models.ChargeStatus chargeStatus) =>
    chargeStatus switch
    {
      Data.Models.ChargeStatus.Added => ChargeStatus.Added,
      Data.Models.ChargeStatus.Removed => ChargeStatus.Removed,
      _ => throw new ArgumentOutOfRangeException(nameof(chargeStatus), chargeStatus, null),
    };
}
