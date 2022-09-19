using DistributedSystemsPatterns.SingleCurrentAggregate.Data.Models;

namespace DistributedSystemsPatterns.SingleCurrentAggregate.Data.Mongo.Mapping;

public static class BillingPeriodStatusMapper
{
  public static BillingPeriodStatus ToDataModel(Models.BillingPeriodStatus billingPeriodStatus) =>
    billingPeriodStatus switch
    {
      Models.BillingPeriodStatus.Open => BillingPeriodStatus.Open,
      Models.BillingPeriodStatus.Closed => BillingPeriodStatus.Closed,
      _ => throw new ArgumentOutOfRangeException(nameof(billingPeriodStatus), billingPeriodStatus, null),
    };

  public static Models.BillingPeriodStatus FromDataModel(BillingPeriodStatus billingPeriodStatus) =>
    billingPeriodStatus switch
    {
      BillingPeriodStatus.Open => Models.BillingPeriodStatus.Open,
      BillingPeriodStatus.Closed => Models.BillingPeriodStatus.Closed,
      _ => throw new ArgumentOutOfRangeException(nameof(billingPeriodStatus), billingPeriodStatus, null),
    };
}
