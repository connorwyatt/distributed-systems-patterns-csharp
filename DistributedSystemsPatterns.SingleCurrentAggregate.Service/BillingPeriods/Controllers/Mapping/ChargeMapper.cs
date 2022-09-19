using DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.Controllers.Models;

namespace DistributedSystemsPatterns.SingleCurrentAggregate.Service.BillingPeriods.Controllers.Mapping;

public static class ChargeMapper
{
  public static Charge ToApiModel(Data.Models.Charge charge) =>
    new(
      charge.ChargeId,
      charge.BillingPeriodId,
      charge.UserId,
      ChargeStatusMapper.ToApiModel(charge.Status),
      charge.Amount,
      charge.AddedAt,
      charge.UpdatedAt);
}
