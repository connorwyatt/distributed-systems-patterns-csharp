using DistributedSystemsPatterns.SingleCurrentAggregate.Data.Models;

namespace DistributedSystemsPatterns.SingleCurrentAggregate.Data.Mongo.Mapping;

public static class BillingPeriodMapper
{
  public static BillingPeriod ToDataModel(Models.BillingPeriod billingPeriod) =>
    new(
      billingPeriod.BillingPeriodId,
      billingPeriod.UserId,
      BillingPeriodStatusMapper.ToDataModel(billingPeriod.Status),
      billingPeriod.TotalAmount,
      billingPeriod.OpenedAt,
      billingPeriod.ClosedAt,
      billingPeriod.UpdatedAt,
      billingPeriod.Version);

  public static Models.BillingPeriod FromDataModel(BillingPeriod billingPeriod) =>
    new(
      billingPeriod.BillingPeriodId,
      billingPeriod.UserId,
      BillingPeriodStatusMapper.FromDataModel(billingPeriod.Status),
      billingPeriod.TotalAmount,
      billingPeriod.OpenedAt,
      billingPeriod.ClosedAt,
      billingPeriod.UpdatedAt,
      billingPeriod.Version);
}
