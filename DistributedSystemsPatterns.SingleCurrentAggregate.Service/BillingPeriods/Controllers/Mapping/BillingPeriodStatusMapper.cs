using DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.Controllers.Models;

namespace DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.Controllers.Mapping;

public static class BillingPeriodStatusMapper
{
  public static BillingPeriodStatus ToApiModel(Data.Models.BillingPeriodStatus billingPeriodStatus) =>
    billingPeriodStatus switch
    {
      Data.Models.BillingPeriodStatus.Open => BillingPeriodStatus.Open,
      Data.Models.BillingPeriodStatus.Closed => BillingPeriodStatus.Closed,
      _ => throw new ArgumentOutOfRangeException(nameof(billingPeriodStatus), billingPeriodStatus, null),
    };

  public static Data.Models.BillingPeriodStatus FromApiModel(BillingPeriodStatus billingPeriodStatus) =>
    billingPeriodStatus switch
    {
      BillingPeriodStatus.Open => Data.Models.BillingPeriodStatus.Open,
      BillingPeriodStatus.Closed => Data.Models.BillingPeriodStatus.Closed,
      _ => throw new ArgumentOutOfRangeException(nameof(billingPeriodStatus), billingPeriodStatus, null),
    };
}
