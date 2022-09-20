using DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.Controllers.Models;

namespace DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.Controllers.Mapping;

public static class BillingPeriodMapper
{
  public static BillingPeriod ToApiModel(Data.Models.BillingPeriod billingPeriod) =>
    new(
      billingPeriod.BillingPeriodId,
      billingPeriod.UserId,
      BillingPeriodStatusMapper.ToApiModel(billingPeriod.Status),
      billingPeriod.TotalAmount,
      billingPeriod.OpenedAt,
      billingPeriod.ClosedAt,
      billingPeriod.UpdatedAt);
}
